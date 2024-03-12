using Autofac;
using Svelto.DataStructures;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Teams;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Utilities;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Teams.Common;
using VoidHuntersRevived.Domain.Teams.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Engines
{
    internal abstract class VoidHuntersEntityDescriptorEngine : BasicEngine, IVoidHuntersEntityDescriptorEngine
    {
        internal static AsyncLocal<uint> EntityId = new AsyncLocal<uint>();

        public abstract VoidHuntersEntityDescriptor Descriptor { get; }

        public abstract EntityInitializer HardSpawn(in VhId sourceEventId, in VhId vhid, in Id<ITeam> teamId, out EntityId id);
        public abstract void SoftSpawn(in VhId sourceEventId, in EntityId id, in GroupIndex groupIndex, ref EntityStatus status);

        public abstract void SoftDespawn(in VhId sourceEventId, in EntityId id, in GroupIndex groupIndex, ref EntityStatus status);

        public abstract void HardDespawn(in VhId sourceEventId, in EntityId id, in GroupIndex groupIndex, ref EntityStatus status);

        public abstract void Serialize(EntityWriter writer, in GroupIndex groupIndex, in SerializationOptions options);

        public abstract void Deserialize(in VhId sourceId, in DeserializationOptions options, EntityReader reader, ref EntityInitializer initializer, in EntityId id);
    }

    internal sealed class VoidHuntersEntityDescriptorEngine<TDescriptor> : VoidHuntersEntityDescriptorEngine, IQueryingEntitiesEngine, IEngineEngine
        where TDescriptor : VoidHuntersEntityDescriptor, new()
    {
        private readonly TDescriptor _descriptor;
        private readonly IEntityFactory _factory;
        private readonly IEntityFunctions _functions;
        private readonly FasterList<ComponentEngineInvoker> _onDespawnEngineInvokers;
        private readonly FasterList<ComponentEngineInvoker> _onSpawnEngineInvokers;
        private readonly FasterList<ComponentSerializer> _serializers;
        private readonly Dictionary<Id<ITeam>, ITeamDescriptorGroup> _teamDescriptorGroups;

        public EntitiesDB entitiesDB { get; set; } = null!;

        public override VoidHuntersEntityDescriptor Descriptor => _descriptor;

        public VoidHuntersEntityDescriptorEngine(
            ITeamDescriptorGroupService teamDescriptorGroups,
            IComponentSerializerService serializers,
            ILifetimeScope scope,
            EnginesRoot enginesRoot,
            IEnumerable<VoidHuntersEntityDescriptor> descriptors)
        {
            _descriptor = descriptors.OfType<TDescriptor>().Single()!;
            _factory = enginesRoot.GenerateEntityFactory();
            _functions = enginesRoot.GenerateEntityFunctions();
            _onDespawnEngineInvokers = new FasterList<ComponentEngineInvoker>();
            _onSpawnEngineInvokers = new FasterList<ComponentEngineInvoker>();
            _teamDescriptorGroups = teamDescriptorGroups.GetAllByDescriptor(_descriptor);
            _serializers = serializers.GetComponentSerializers(_descriptor);
        }

        public void Initialize(IEngine[] engines)
        {
            foreach (Type componentType in _descriptor.componentsToBuild.Select(x => x.GetEntityComponentType()))
            {
                if (ComponentEngineInvoker.Create(typeof(OnDespawnEngineInvoker<>), typeof(IOnDespawnEngine<>), componentType, engines, out var invoker))
                {
                    _onDespawnEngineInvokers.Add(invoker);
                }

                if (ComponentEngineInvoker.Create(typeof(OnSpawnEngineInvoker<>), typeof(IOnSpawnEngine<>), componentType, engines, out invoker))
                {
                    _onSpawnEngineInvokers.Add(invoker);
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

        public override void Deserialize(in VhId sourceId, in DeserializationOptions options, EntityReader reader, ref EntityInitializer initializer, in EntityId id)
        {
            foreach (ComponentSerializer serializer in _serializers)
            {
                serializer.Deserialize(in sourceId, in options, reader, ref initializer, in id);
            }
        }

        public override EntityInitializer HardSpawn(in VhId sourceEventId, in VhId vhid, in Id<ITeam> teamId, out EntityId id)
        {
            EGID egid = new EGID(EntityId.Value++, _teamDescriptorGroups[teamId].GroupId);
            id = new EntityId(egid, vhid);

            EntityInitializer initializer = _factory.BuildEntity(egid, _descriptor);
            initializer.Init(id);
            initializer.Init(_descriptor.Id);
            initializer.Init(teamId);

            return initializer;
        }

        public override void SoftSpawn(in VhId sourceEventId, in EntityId id, in GroupIndex groupIndex, ref EntityStatus status)
        {
            for (int i = 0; i < _onSpawnEngineInvokers.count; i++)
            {
                _onSpawnEngineInvokers[i].Invoke(sourceEventId, entitiesDB, id, groupIndex);
            }
        }

        public override void SoftDespawn(in VhId sourceEventId, in EntityId id, in GroupIndex groupIndex, ref EntityStatus status)
        {
            for (int i = 0; i < _onDespawnEngineInvokers.count; i++)
            {
                _onDespawnEngineInvokers[i].Invoke(sourceEventId, entitiesDB, id, groupIndex);
            }
        }

        public override void HardDespawn(in VhId sourceEventId, in EntityId id, in GroupIndex groupIndex, ref EntityStatus status)
        {
            _functions.RemoveEntity<TDescriptor>(id.EGID);
        }
    }
}
