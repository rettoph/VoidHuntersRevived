using Svelto.ECS;

namespace VoidHuntersRevived.Common.Entities.Components
{
    public struct StaticEntity : IEntityComponent
    {
        public static readonly FilterContextID InstanceEntitiesFilterContextId = FilterContextID.GetNewContextID();

        public readonly CombinedFilterID InstanceEntitiesFilterId;

        public int InstanceEntitiesCount;

        public StaticEntity(CombinedFilterID instanceEntitiesFilterId)
        {
            this.InstanceEntitiesCount = 0;
            this.InstanceEntitiesFilterId = instanceEntitiesFilterId;
        }
    }
}
