﻿using Guppy.Resources.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Physics.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Common.Components.Instance
{
    [PolymorphicJsonType<IPieceComponent>(nameof(Plug))]
    public struct Plug : IEntityComponent, IPieceComponent
    {
        public static readonly Plug Default = new Plug()
        {
            Location = new Location(FixVector2.UnitX / (Fix64)2, Fix64.PiOver2)
        };

        public required Location Location { get; init; }
    }
}
