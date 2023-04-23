﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Services
{
    public interface ITractorService
    {
        bool TryGetTractorable(
            Pilotable pilotable, 
            [MaybeNullWhen(false)] out ParallelKey tractorableKey,
            [MaybeNullWhen(false)] out ParallelKey nodeKey);
        
        bool CanTractor(Vector2 target, ParallelKey tractorable);

        bool TryGetPotentialEdge(
            Vector2 target,
            int tractoringId,
            [MaybeNullWhen(false)] out Vector2 position,
            [MaybeNullWhen(false)] out Degree outDegree);

        bool TransformTractorable(
            Vector2 target,
            Tractoring tractoring,
            [MaybeNullWhen(false)] out Edge potential);

        bool TransformTractorable(
            Vector2 target,
            int tractoringId,
            int tractorableId,
            [MaybeNullWhen(false)] out Edge potential);
    }
}
