using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common.Components
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
