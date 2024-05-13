using Svelto.ECS;

namespace VoidHuntersRevived.Common.Entities.Components
{
    public struct StaticData : IEntityComponent
    {
        public static readonly FilterContextID InstanceEntitiesFilterContextId = FilterContextID.GetNewContextID();

        public readonly CombinedFilterID InstanceEntitiesFilterId;

        public int InstanceEntitiesCount;

        public StaticData(CombinedFilterID instanceEntitiesFilterId)
        {
            this.InstanceEntitiesCount = 0;
            this.InstanceEntitiesFilterId = instanceEntitiesFilterId;
        }
    }
}
