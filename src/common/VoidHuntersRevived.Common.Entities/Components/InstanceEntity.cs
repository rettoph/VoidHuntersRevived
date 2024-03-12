using Svelto.ECS;

namespace VoidHuntersRevived.Common.Entities.Components
{
    public struct InstanceEntity : IEntityComponent
    {
        public readonly GroupIndex StaticEntityId;

        public InstanceEntity(GroupIndex staticEntity) : this()
        {
            this.StaticEntityId = staticEntity;
        }
    }
}
