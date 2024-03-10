using Svelto.ECS;
using VoidHuntersRevived.Common.Pieces.Enums;

namespace VoidHuntersRevived.Common.Ships.Components
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
