using Guppy.Attributes;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Initializers;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Domain.Entities.Utilities;

namespace VoidHuntersRevived.Domain.Entities.Engines
{
    [AutoLoad]
    internal sealed class StaticEntityEngine : BasicEngine, IQueryingEntitiesEngine, IReactOnAddEx<InstanceEntity>, IReactOnAddEx<StaticEntity>
    {
        private readonly IEntityTypeService _types;

        public EntitiesDB entitiesDB { get; set; } = default!;

        public StaticEntityEngine(EnginesRoot enginesRoot, IEntityTypeInitializerService entityTypeInitializers, IEntityTypeService types, SimpleEntitiesSubmissionScheduler scheduler)
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

            scheduler.SubmitEntities();
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
                ref var filter = ref this.entitiesDB.GetFilters().GetOrCreatePersistentFilter<EntityId>(instances[i].FilterId);
                filter.Add(ids[i], groupID, i);
            }
        }
    }
}
