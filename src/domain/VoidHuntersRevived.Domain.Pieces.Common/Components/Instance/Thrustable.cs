using Guppy.Resources.Attributes;
using Svelto.ECS;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Pieces.Common.Enums;

namespace VoidHuntersRevived.Domain.Pieces.Common.Components.Instance
{
    [PolymorphicJsonType<IPieceComponent>(nameof(Thrustable))]
    public struct Thrustable : IEntityComponent, IPieceComponent
    {
        [JsonIgnore]
        public Direction Direction;

        public FixPolar MaxImpulse { get; init; }

        public FixVector2 ImpulsePoint { get; init; }
    }
}
