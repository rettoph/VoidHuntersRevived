using Guppy.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Domain.Entities.Common.Initializers;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Utilities;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Entities.Engines
{
    [AutoLoad]
    internal sealed class StaticEntityEngine : BasicEngine, IQueryingEntitiesEngine, IReactOnAddEx<InstanceEntity>, IReactOnAddEx<StaticEntity>
    {
        private readonly IEntityTypeService _types;

        public EntitiesDB entitiesDB { get; set; } = default!;

        public StaticEntityEngine(EnginesRoot enginesRoot, IEntityTypeInitializerService entityTypeInitializers, IEntityTypeService types)
        {
            _types = types;

            // Automatically create all static entities
            IEntityFactory factory = enginesRoot.GenerateEntityFactory();

            foreach (IEntityTypeInitializer typeInitializer in entityTypeInitializers.GetAll())
            {
                var data = StaticEntityHelper.GetData(typeInitializer.Type);

                EntityInitializer entityInitializer = factory.BuildEntity(data.EGID, typeInitializer.Type.Descriptor.StaticDescriptor);
                entityInitializer.Init(data.StaticComponent);
                entityInitializer.Init(typeInitializer.Type.Id);
                entityInitializer.Init(typeInitializer.Type.Descriptor.Id);

                typeInitializer.InitializeStatic(ref entityInitializer);
            }
        }

        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<StaticEntity> entities, ExclusiveGroupStruct groupID)
        {
            var (instances, ids, _) = entities;
            var (typeIds, _) = this.entitiesDB.QueryEntities<Id<IEntityType>>(groupID);

            for (uint i = rangeOfEntities.start; i < rangeOfEntities.end; i++)
            {
                Id<IEntityType> typeId = typeIds[i];
                IEntityType type = _types.GetById(typeId);

                StaticEntityHelper.SetGroupIndex(type, new GroupIndex(groupID, i));
            }
        }

        /// <summary>
        /// Automatically add instance entities to their respective
        /// static filters.
        /// </summary>
        /// <param name="rangeOfEntities"></param>
        /// <param name="entities"></param>
        /// <param name="groupID"></param>
        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<InstanceEntity> entities, ExclusiveGroupStruct groupID)
        {
            var (instances, ids, _) = entities;

            for (uint i = rangeOfEntities.start; i < rangeOfEntities.end; i++)
            {
                InstanceEntity instanceComponent = instances[i];
                ref StaticEntity staticComponent = ref this.entitiesDB.QueryEntityByIndex<StaticEntity>(instanceComponent.StaticEntityId.Index, instanceComponent.StaticEntityId.GroupID);

                ref var filter = ref this.entitiesDB.GetFilters().GetOrCreatePersistentFilter<EntityId>(staticComponent.InstanceEntitiesFilterId);
                filter.Add(ids[i], groupID, i);

                staticComponent.InstanceEntitiesCount++;
            }
        }
    }
}
