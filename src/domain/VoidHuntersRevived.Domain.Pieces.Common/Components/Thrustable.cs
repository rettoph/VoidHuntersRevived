using System.Text.Json.Serialization;
using Svelto.ECS;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Enums;

namespace VoidHuntersRevived.Domain.Pieces.Common.Components
{
    public struct Thrustable : IEntityComponent, IPieceComponent, ICompositeBelongsTo<Body, Fixture, Thrustable>
    {
        [JsonIgnore]
        public Direction Direction;

        public FixPolar MaxImpulse { get; init; }

        public FixVector2 ImpulsePoint { get; init; }
    }
}