using Svelto.ECS;
using VoidHuntersRevived.Common.FixedPoint;

namespace VoidHuntersRevived.Common.Entities.Components
{
    public unsafe struct Transform2D : IEntityComponent
    {
        public FixTransform2D Value;
    }
}
