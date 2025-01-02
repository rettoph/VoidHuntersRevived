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
            return new EntityFilterId<TChild>(egid, new(unchecked((int)egid.entityID), FilterContextHelper.GetFilterContext<TParent, TChild>()));
        }
        public static EntityFilterId<TChild> Create<TParent>(EntityLocalId localId)
            where TParent : unmanaged, IEntityComponent
        {
            return EntityFilterId<TChild>.Create<TParent>(localId.Value);
        }

        public override bool Equals(object? obj)
        {
            return obj is EntityFilterId<TChild> id
                && CombinedFilterId.filterID == id.CombinedFilterId.filterID
                && CombinedFilterId.contextID.id == id.CombinedFilterId.contextID.id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CombinedFilterId);
        }

        public static bool operator ==(EntityFilterId<TChild> f1, EntityFilterId<TChild> f2)
        {
            return f1.CombinedFilterId.filterID == f2.CombinedFilterId.filterID
                && f1.CombinedFilterId.contextID.id == f2.CombinedFilterId.contextID.id;
        }

        public static bool operator !=(EntityFilterId<TChild> f1, EntityFilterId<TChild> f2)
        {
            return f1.CombinedFilterId.filterID != f2.CombinedFilterId.filterID
                || f1.CombinedFilterId.contextID.id != f2.CombinedFilterId.contextID.id;
        }
    }
}
