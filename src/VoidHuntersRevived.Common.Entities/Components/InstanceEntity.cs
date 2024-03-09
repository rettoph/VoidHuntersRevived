using Svelto.ECS;

namespace VoidHuntersRevived.Common.Entities.Components
{
    public struct InstanceEntity : IEntityComponent
    {
        public readonly EGID StaticEntity;
        public readonly CombinedFilterID FilterId;

        public InstanceEntity(EGID staticEgid, CombinedFilterID staticEntityInstanceEntitiesFilterId) : this()
        {
            this.StaticEntity = staticEgid;
            this.FilterId = staticEntityInstanceEntitiesFilterId;
        }
    }
}
