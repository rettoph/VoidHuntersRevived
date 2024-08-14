using Svelto.ECS;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;

namespace VoidHuntersRevived.Domain.Graphics.Common.Components
{
    public struct PrimitiveInstance : IEntityComponent
    {
        public readonly PrimitiveGroupEnum Group;

        public PrimitiveInstance(PrimitiveGroupEnum group)
        {
            this.Group = group;
        }
    }
}
