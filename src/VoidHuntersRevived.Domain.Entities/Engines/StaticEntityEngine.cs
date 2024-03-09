using Guppy.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Initializers;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Simulations.Engines;

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

        public StaticEntityEngine(EnginesRoot enginesRoot, IEntityTypeInitializerService entityTypeInitializers)
        {
            IEntityFactory factory = enginesRoot.GenerateEntityFactory();

            uint id = 0;
            foreach (IEntityTypeInitializer typeInitializer in entityTypeInitializers.GetAll())
            {
                EGID staticEgid = new EGID(id++, GetExclusiveGroupStruct(typeInitializer.Type.Descriptor));
                CombinedFilterID instanceEntitiesFilterId = new CombinedFilterID((int)id, StaticEntity.InstanceEntitiesFilterContextId);

                EntityInitializer entityInitializer = factory.BuildEntity(staticEgid, typeInitializer.Type.Descriptor.StaticDescriptor);
                entityInitializer.Init(new StaticEntity(instanceEntitiesFilterId));

                typeInitializer.InitializeStatic(ref entityInitializer);
                typeInitializer.InstanceEntityInitializer += (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
                {
                    initializer.Init(new InstanceEntity(staticEgid, instanceEntitiesFilterId));
                };
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
