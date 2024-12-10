using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common.Utilities
{
    public readonly struct EntityFilterId<T>(EGID id, FilterContextID? context = null)
        where T : unmanaged, IEntityComponent
    {
        private static class FilterContext<TFilter, TId>
        {
            public static readonly FilterContextID Value = FilterContextID.GetNewContextID();
        }

        public readonly EGID Id = id;
        public readonly CombinedFilterID CombinedFilterId = new(unchecked((int)id.entityID), context ?? FilterContext<T, EntityId>.Value);

        public static FilterContextID GetFilterContext<TId>()
        {
            return FilterContext<T, TId>.Value;
        }

        public override bool Equals(object? obj)
        {
            return obj is EntityFilterId<T> id
                && this.CombinedFilterId.filterID == id.CombinedFilterId.filterID
                && this.CombinedFilterId.contextID.id == id.CombinedFilterId.contextID.id;
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
