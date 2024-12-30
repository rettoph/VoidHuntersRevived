using Svelto.ECS;
using VoidHuntersRevived.Domain.Pieces.Common.Enums;

namespace VoidHuntersRevived.Domain.Ships.Common.Components
{
    public struct Helm() : IEntityComponent
    {
        public Direction Direction = Direction.None;
    }
}
