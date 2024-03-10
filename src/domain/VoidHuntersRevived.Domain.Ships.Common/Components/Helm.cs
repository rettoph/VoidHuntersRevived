using Svelto.ECS;
using VoidHuntersRevived.Domain.Pieces.Common.Enums;

namespace VoidHuntersRevived.Domain.Ships.Common.Components
{
    public struct Helm : IEntityComponent
    {
        public static readonly FilterContextID ThrustableFilterContextId = FilterContextID.GetNewContextID();

        public Direction Direction;

        public Helm()
        {
            this.Direction = Direction.None;
        }
    }
}
