using Svelto.ECS;

namespace VoidHuntersRevived.Common.Entities.Components
{
    public struct InstanceEntity : IEntityComponent
    {
        public readonly GroupIndex StaticEntity;
        public readonly CombinedFilterID FilterId;

        public InstanceEntity(GroupIndex staticEntity, CombinedFilterID staticEntityInstanceEntitiesFilterId) : this()
        {
            this.StaticEntity = staticEntity;
            this.FilterId = staticEntityInstanceEntitiesFilterId;
        }
    }
}
