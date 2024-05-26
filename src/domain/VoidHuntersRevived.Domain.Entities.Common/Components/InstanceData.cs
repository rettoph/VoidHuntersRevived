using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common.Components
{
    public struct InstanceData : IEntityComponent
    {
        public readonly GroupIndex StaticEntityId;

        public InstanceData(GroupIndex staticEntity) : this()
        {
            this.StaticEntityId = staticEntity;
        }
    }
}
