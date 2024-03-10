using Guppy.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Initializers;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Domain.Entities.Utilities;

namespace VoidHuntersRevived.Domain.Entities.Engines
{
    [AutoLoad]
    internal sealed class StaticEntityEngine : BasicEngine, IQueryingEntitiesEngine, IReactOnAddEx<InstanceEntity>
    {
        public EntitiesDB entitiesDB { get; set; } = default!;

        public StaticEntityEngine(EnginesRoot enginesRoot, IEntityTypeInitializerService entityTypeInitializers)
        {
            // Automatically create all static entities
            IEntityFactory factory = enginesRoot.GenerateEntityFactory();

            foreach (IEntityTypeInitializer typeInitializer in entityTypeInitializers.GetAll())
            {
                var (egid, @static, _) = StaticEntityHelper.GetComponents(typeInitializer.Type);

                EntityInitializer entityInitializer = factory.BuildEntity(egid, typeInitializer.Type.Descriptor.StaticDescriptor);
                entityInitializer.Init(@static);

                typeInitializer.InitializeStatic(ref entityInitializer);
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
