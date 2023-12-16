using Guppy.Resources.Attributes;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces.Enums;

namespace VoidHuntersRevived.Common.Pieces.Components
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
