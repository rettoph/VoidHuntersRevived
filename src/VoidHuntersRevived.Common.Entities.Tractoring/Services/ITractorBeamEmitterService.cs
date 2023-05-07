using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.Tractoring.Components;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities.Tractoring.Services
{
    public interface ITractorBeamEmitterService
    {
        bool TryGetTractorable(
            int tractorBeamEmitterId,
            [MaybeNullWhen(false)] out ParallelKey tractorableKey,
            [MaybeNullWhen(false)] out ParallelKey nodeKey);

        bool TryGetPotentialLink(
            int tractorBeamEmitterId,
            [MaybeNullWhen(false)] out Vector2 position,
            [MaybeNullWhen(false)] out Joint childJoint);

        bool TransformTractorable(
            int tractorBeamEmitterId,
            [MaybeNullWhen(false)] out Link potential);
    }
}
