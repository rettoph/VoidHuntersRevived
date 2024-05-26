using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common.Components
{
    public struct TypeData : IEntityComponent
    {
        public static readonly FilterContextID InstanceEntitiesFilterContextId = FilterContextID.GetNewContextID();

        public readonly CombinedFilterID InstanceEntitiesFilterId;

        public int InstanceEntitiesCount;

        public TypeData(CombinedFilterID instanceEntitiesFilterId)
        {
            this.InstanceEntitiesCount = 0;
            this.InstanceEntitiesFilterId = instanceEntitiesFilterId;
        }
    }
}
