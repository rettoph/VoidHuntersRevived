using Svelto.ECS;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Utilities
{
    public readonly struct FilterVhId<T>
        where T : unmanaged, IEntityComponent
    {
        private static readonly int _filterId;
        private static class FilterContext<TFilter, TId>
        {
            public static readonly FilterContextID Value = FilterContextID.GetNewContextID();
        }

        public readonly VhId VhId;
        public readonly CombinedFilterID CombinedFilterId;

        public FilterVhId(EntityId id, FilterContextID? context = null)
        {
            this.VhId = id.VhId;
            this.CombinedFilterId = new CombinedFilterID(unchecked((int)id.EGID.entityID), context ?? FilterContext<T, EntityId>.Value);
        }

        public static FilterContextID GetFilterContext<TId>()
        {
            return FilterContext<T, TId>.Value;
        }

        public override bool Equals(object? obj)
        {
            return obj is FilterVhId<T> id
                && this.CombinedFilterId.filterID == id.CombinedFilterId.filterID
                && this.CombinedFilterId.contextID.id == id.CombinedFilterId.contextID.id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CombinedFilterId);
        }

        public static bool operator ==(FilterVhId<T> f1, FilterVhId<T> f2)
        {
            return f1.CombinedFilterId.filterID == f2.CombinedFilterId.filterID
                && f1.CombinedFilterId.contextID.id == f2.CombinedFilterId.contextID.id;
        }

        public static bool operator !=(FilterVhId<T> f1, FilterVhId<T> f2)
        {
            return f1.CombinedFilterId.filterID != f2.CombinedFilterId.filterID
                || f1.CombinedFilterId.contextID.id != f2.CombinedFilterId.contextID.id;
        }
    }
}
