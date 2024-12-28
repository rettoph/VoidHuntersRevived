using Svelto.ECS;
using VoidHuntersRevived.Common.FixedPoint;

namespace VoidHuntersRevived.Domain.Pieces.Common.Components
{
    public readonly struct Plug : IEntityComponent, IPieceComponent
    {
        public static readonly Plug Default = new()
        {
            NodeTransform = new FixTransform2D(FixVector2.UnitX / (Fix64)2, Fix64.PiOver2)
        };

        public required FixTransform2D NodeTransform { get; init; }
    }
}
