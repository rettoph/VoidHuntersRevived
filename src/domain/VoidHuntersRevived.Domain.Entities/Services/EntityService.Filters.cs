using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal partial class EntityService
    {
        public ref EntityFilterCollection GetFilter<T>(EntityId id, FilterContextID filterContext)
            where T : unmanaged, IEntityComponent
        {
            ref var filter = ref this.entitiesDB.GetFilters().GetOrCreatePersistentFilter<T>(unchecked((int)id.EGID.entityID), filterContext);

            return ref filter;
        }

        public ref EntityFilterCollection GetFilter<T>(CombinedFilterID filterId)
            where T : unmanaged, IEntityComponent
        {
            ref var filter = ref this.entitiesDB.GetFilters().GetOrCreatePersistentFilter<T>(filterId);

            return ref filter;
        }
    }
}
