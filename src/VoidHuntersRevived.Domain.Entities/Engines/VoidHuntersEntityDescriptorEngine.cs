using Autofac;
using Svelto.DataStructures;
using Svelto.ECS;
using System.Text.RegularExpressions;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Enums;
using VoidHuntersRevived.Common.Entities.Options;
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

        public abstract EntityInitializer Spawn(in VhId vhid, in Id<ITeam> teamId, out EntityId id);

        public abstract void SoftDespawn(in EntityId id, in GroupIndex groupIndex);
        public abstract void RevertSoftDespawn(in EntityId id, in GroupIndex groupIndex);

        public abstract void HardDespawn(in EntityId id, in GroupIndex groupIndex);

        public abstract void Serialize(EntityWriter writer, in GroupIndex groupIndex, in SerializationOptions options);

        public abstract void Deserialize(in DeserializationOptions options, EntityReader reader, ref EntityInitializer initializer, in EntityId id);
    }

    internal sealed class VoidHuntersEntityDescriptorEngine<TDescriptor> : VoidHuntersEntityDescriptorEngine, IQueryingEntitiesEngine
        where TDescriptor : VoidHuntersEntityDescriptor, new()
    {
        private readonly TDescriptor _descriptor;
        private readonly IEntityFactory _factory;
        private readonly IEntityFunctions _functions;
        private readonly IEngineService _engines;
        private readonly FasterList<OnDespawnEngineInvoker> _onDespawnEngineInvokers;
        private readonly FasterList<ComponentSerializer> _serializers;
        private readonly Dictionary<Id<ITeam>, ITeamDescriptorGroup> _teamDescriptorGroups;

        public EntitiesDB entitiesDB { get; set; } = null!;

        public override VoidHuntersEntityDescriptor Descriptor => _descriptor;

        public VoidHuntersEntityDescriptorEngine(
            ITeamDescriptorGroupService teamDescriptorGroups,
            IEngineService engines, 
            ILifetimeScope scope, 
            EnginesRoot enginesRoot, 
            IEnumerable<VoidHuntersEntityDescriptor> descriptors)
        {
            _descriptor = descriptors.OfType<TDescriptor>().Single()!;
            _factory = enginesRoot.GenerateEntityFactory();
            _functions = enginesRoot.GenerateEntityFunctions();
            _engines = engines;
            _onDespawnEngineInvokers = new FasterList<OnDespawnEngineInvoker>();
            _teamDescriptorGroups = teamDescriptorGroups.GetAllByDescriptor(_descriptor);

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

        public override void Serialize(EntityWriter writer, in GroupIndex groupIndex, in SerializationOptions options)
        {
            foreach (ComponentSerializer serializer in _serializers)
            {
                serializer.Serialize(writer, in groupIndex, entitiesDB, in options);
            }
        }

        public override void Deserialize(in DeserializationOptions options, EntityReader reader, ref EntityInitializer initializer, in EntityId id)
        {
            foreach (ComponentSerializer serializer in _serializers)
            {
                serializer.Deserialize(in options, reader, ref initializer, in id);
            }
        }

        public override EntityInitializer Spawn(in VhId vhid, in Id<ITeam> teamId, out EntityId id)
        {
            EGID egid = new EGID(EntityId++, _teamDescriptorGroups[teamId].GroupId);
            id = new EntityId(egid, vhid);

            EntityInitializer initializer = _factory.BuildEntity(egid, _descriptor);
            initializer.Init(id);
            initializer.Init(_descriptor.Id);
            initializer.Init(teamId);
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
