using Svelto.ECS;

namespace VoidHuntersRevived.Common.Entities.Components
{
    public struct StaticEntity : IEntityComponent
    {
        public static readonly FilterContextID InstanceEntitiesFilterContextId = FilterContextID.GetNewContextID();

        public readonly CombinedFilterID InstanceEntitiesFilterId;

        public StaticEntity(CombinedFilterID instanceEntitiesFilterId)
        {
            InstanceEntitiesFilterId = instanceEntitiesFilterId;
        }
    }
}
