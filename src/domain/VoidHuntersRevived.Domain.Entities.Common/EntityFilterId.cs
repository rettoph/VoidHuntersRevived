using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Utilities;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public readonly struct EntityFilterId<T>(EGID id, FilterContextID? context = null)
        where T : unmanaged, IEntityComponent
    {
        public readonly EGID Id = id;
        public readonly CombinedFilterID CombinedFilterId = new(unchecked((int)id.entityID), context ?? FilterContextHelper.GetFilterContext<T, EGID>());

        public override bool Equals(object? obj)
        {
            return obj is EntityFilterId<T> id
                && CombinedFilterId.filterID == id.CombinedFilterId.filterID
                && CombinedFilterId.contextID.id == id.CombinedFilterId.contextID.id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CombinedFilterId);
        }

        public static bool operator ==(EntityFilterId<T> f1, EntityFilterId<T> f2)
        {
            return f1.CombinedFilterId.filterID == f2.CombinedFilterId.filterID
                && f1.CombinedFilterId.contextID.id == f2.CombinedFilterId.contextID.id;
        }

        public static bool operator !=(EntityFilterId<T> f1, EntityFilterId<T> f2)
        {
            return f1.CombinedFilterId.filterID != f2.CombinedFilterId.filterID
                || f1.CombinedFilterId.contextID.id != f2.CombinedFilterId.contextID.id;
        }
    }
}
