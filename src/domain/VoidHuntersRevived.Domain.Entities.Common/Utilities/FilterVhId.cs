using Svelto.ECS;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Utilities
{
    public readonly struct FilterVhId<T>(EntityId id, FilterContextID? context = null)
        where T : unmanaged, IEntityComponent
    {
        private static class FilterContext<TFilter, TId>
        {
            public static readonly FilterContextID Value = FilterContextID.GetNewContextID();
        }

        public readonly VhId VhId = id.VhId;
        public readonly CombinedFilterID CombinedFilterId = new(unchecked((int)id.EGID.entityID), context ?? FilterContext<T, EntityId>.Value);

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
