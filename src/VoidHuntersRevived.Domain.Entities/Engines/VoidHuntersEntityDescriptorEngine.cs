using Autofac;
using Svelto.DataStructures;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Enums;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Domain.Entities.Utilities;

namespace VoidHuntersRevived.Domain.Entities.Engines
{
    internal abstract class VoidHuntersEntityDescriptorEngine : IVoidHuntersEntityDescriptorEngine
    {
        [ThreadStatic]
        internal static uint EntityId;

        public abstract VoidHuntersEntityDescriptor Descriptor { get; }

        public abstract EntityInitializer Spawn(VhId vhid, out EntityId id);

        public abstract void SoftDespawn(in EntityId id, in GroupIndex groupIndex);
        public abstract void RevertSoftDespawn(in EntityId id, in GroupIndex groupIndex);

        public abstract void HardDespawn(in EntityId id, in GroupIndex groupIndex);

        public abstract void Serialize(EntityWriter writer, in GroupIndex groupIndex);

        public abstract void Deserialize(EntityReader reader, ref EntityInitializer initializer, in EntityId id);
    }

    internal sealed class VoidHuntersEntityDescriptorEngine<TDescriptor> : VoidHuntersEntityDescriptorEngine, IQueryingEntitiesEngine
        where TDescriptor : VoidHuntersEntityDescriptor, new()
    {
        private readonly DescriptorId _descriptorComponent;
        private readonly TDescriptor _descriptor;
        private readonly IEntityFactory _factory;
        private readonly IEntityFunctions _functions;
        private readonly IEngineService _engines;
        private readonly ExclusiveGroup _group;
        private readonly FasterList<OnDespawnEngineInvoker> _onDespawnEngineInvokers;
        private readonly FasterList<ComponentSerializer> _serializers;

        public EntitiesDB entitiesDB { get; set; } = null!;

        public override VoidHuntersEntityDescriptor Descriptor => _descriptor;

        public VoidHuntersEntityDescriptorEngine(IEngineService engines, ILifetimeScope scope, EnginesRoot enginesRoot, IEnumerable<VoidHuntersEntityDescriptor> descriptors)
        {
            _descriptor = descriptors.OfType<TDescriptor>().Single()!;
            _factory = enginesRoot.GenerateEntityFactory();
            _functions = enginesRoot.GenerateEntityFunctions();
            _engines = engines;
            _onDespawnEngineInvokers = new FasterList<OnDespawnEngineInvoker>();
            _group = new ExclusiveGroup();
            _descriptorComponent = new DescriptorId(_descriptor.Id);

            _serializers = new FasterList<ComponentSerializer>(_descriptor.ComponentManagers.Count());
            foreach (ComponentManager manager in _descriptor.ComponentManagers)
            {
                _serializers.Add(manager.SerializerFactory.Create(scope));
            }
        }

        public void Ready()
        {
            var engines = _engines.All();
            
            foreach (Type componentType in _descriptor.ComponentManagers.Select(x => x.Type))
            {
                if (OnDespawnEngineInvoker.Create(componentType, engines, out var invoker))
                {
                    _onDespawnEngineInvokers.Add(invoker);
                }
            }
        }

        public override void Serialize(EntityWriter writer, in GroupIndex groupIndex)
        {
            foreach (ComponentSerializer serializer in _serializers)
            {
                serializer.Serialize(writer, groupIndex.Index, groupIndex.GroupID, entitiesDB);
            }
        }

        public override void Deserialize(EntityReader reader, ref EntityInitializer initializer, in EntityId id)
        {
            foreach (ComponentSerializer serializer in _serializers)
            {
                serializer.Deserialize(reader, ref initializer, in id);
            }
        }

        public override EntityInitializer Spawn(VhId vhid, out EntityId id)
        {
            EGID egid = new EGID(EntityId++, _group);
            id = new EntityId(egid, vhid);

            EntityInitializer initializer = _factory.BuildEntity(egid, _descriptor);
            initializer.Init(id);
            initializer.Init(_descriptorComponent);
            initializer.Init(new EntityStatus()
            {
                Value = EntityStatusEnum.Spawned
            });

            return initializer;
        }

        public override void SoftDespawn(in EntityId id, in GroupIndex groupIndex)
        {
            ref var status = ref entitiesDB.QueryEntityByIndex<EntityStatus>(groupIndex.Index, groupIndex.GroupID);
            status.Value = EntityStatusEnum.SoftDespawned;

            for (int i = 0; i < _onDespawnEngineInvokers.count; i++)
            {
                _onDespawnEngineInvokers[i].Invoke(entitiesDB, id, groupIndex);
            }
        }
        public override void RevertSoftDespawn(in EntityId id, in GroupIndex groupIndex)
        {
            ref var status = ref entitiesDB.QueryEntityByIndex<EntityStatus>(groupIndex.Index, groupIndex.GroupID);
            status.Value = EntityStatusEnum.Spawned;
        }

        public override void HardDespawn(in EntityId id, in GroupIndex groupIndex)
        {
            ref var status = ref entitiesDB.QueryEntityByIndex<EntityStatus>(groupIndex.Index, groupIndex.GroupID);
            status.Value = EntityStatusEnum.HardDespawned;

            _functions.RemoveEntity<TDescriptor>(id.EGID);
        }
    }
}
