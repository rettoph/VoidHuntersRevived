using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Utilities;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public readonly struct EntityFilterId<TChild>
        where TChild : unmanaged, IEntityComponent
    {
        public readonly EGID EGID;
        public readonly CombinedFilterID CombinedFilterId;

        private EntityFilterId(EGID egid, CombinedFilterID combinedFilterId)
        {
            this.EGID = egid;
            this.CombinedFilterId = combinedFilterId;
        }

        private static class FilterContextId<TParent>
            where TParent : unmanaged, IEntityComponent
        {
            public static readonly FilterContextID Value = FilterContextHelper.GetFilterContext<TParent, TChild>();
        }

        public bool IsDefault<TParent>()
            where TParent : unmanaged, IEntityComponent
        {
            return this.EGID == default && this.CombinedFilterId.contextID.id == FilterContextId<TParent>.Value.id;
        }

        public static EntityFilterId<TChild> Create<TParent>(EGID egid)
            where TParent : unmanaged, IEntityComponent
        {
            return new(egid, new(unchecked((int)egid.entityID), FilterContextHelper.GetFilterContext<TParent, TChild>()));
        }

        public static EntityFilterId<TChild> Create<TParent>(EntityLocalId localId)
                    where TParent : unmanaged, IEntityComponent
        {
            return EntityFilterId<TChild>.Create<TParent>(localId.Value);
        }

        public override bool Equals(object? obj)
        {
            return obj is EntityFilterId<TChild> id
                && this.CombinedFilterId.filterID == id.CombinedFilterId.filterID
                && this.CombinedFilterId.contextID.id == id.CombinedFilterId.contextID.id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.CombinedFilterId);
        }

        public static bool operator ==(EntityFilterId<TChild> left, EntityFilterId<TChild> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EntityFilterId<TChild> left, EntityFilterId<TChild> right)
        {
            return !(left == right);
        }
    }
}