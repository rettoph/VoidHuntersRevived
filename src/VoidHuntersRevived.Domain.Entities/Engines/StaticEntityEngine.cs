using Guppy.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Domain.Entities.Services;

namespace VoidHuntersRevived.Domain.Entities.Engines
{
    [AutoLoad]
    internal sealed class StaticEntityEngine : BasicEngine, IQueryingEntitiesEngine, IReactOnAddEx<InstanceEntity>
    {
        private static Dictionary<string, ExclusiveGroup> _exclusiveGroups = new Dictionary<string, ExclusiveGroup>();
        private static ExclusiveGroupStruct GetExclusiveGroupStruct(VoidHuntersEntityDescriptor descriptor)
        {
            if (_exclusiveGroups.TryGetValue(descriptor.Name, out ExclusiveGroup? exclusiveGroup))
            {
                return exclusiveGroup;
            }

            exclusiveGroup = new ExclusiveGroup($"{descriptor.Name}.Static");
            _exclusiveGroups.Add(descriptor.Name, exclusiveGroup);

            return exclusiveGroup;
        }

        public EntitiesDB entitiesDB { get; set; } = default!;

        public StaticEntityEngine(EnginesRoot enginesRoot, EntityTypeService entityTypes)
        {
            IEntityFactory factory = enginesRoot.GenerateEntityFactory();

            uint id = 0;
            foreach (IEntityType type in entityTypes.GetAll())
            {
                EGID staticEgid = new EGID(id++, GetExclusiveGroupStruct(type.Descriptor));
                CombinedFilterID instanceEntitiesFilterId = new CombinedFilterID((int)id, StaticEntity.InstanceEntitiesFilterContextId);

                EntityInitializer initializer = factory.BuildEntity(staticEgid, type.Descriptor.StaticDescriptor);
                initializer.Init(new StaticEntity(instanceEntitiesFilterId));

                IEntityTypeConfiguration configuration = entityTypes.GetConfiguration(type);

                configuration.InitializeStatic(ref initializer);
                configuration.InitializeInstanceComponent<InstanceEntity>(new InstanceEntity(staticEgid, instanceEntitiesFilterId));
            }
        }

        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<InstanceEntity> entities, ExclusiveGroupStruct groupID)
        {
            var (instances, ids, _) = entities;

            for (uint i = rangeOfEntities.start; i < rangeOfEntities.end; i++)
            {
                ref var filter = ref this.entitiesDB.GetFilters().GetOrCreatePersistentFilter<EntityId>(instances[i].FilterId);
                filter.Add(ids[i], groupID, i);
            }
        }
    }
}
