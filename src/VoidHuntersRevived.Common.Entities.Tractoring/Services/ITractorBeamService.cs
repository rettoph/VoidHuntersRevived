using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.Tractoring.Components;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities.Tractoring.Services
{
    public interface ITractorBeamService
    {
        void Update(int emitterId);

        bool Query(
            ISimulation simulation,
            int emitterId,
            out int targetNodeId);

        bool TryGetPotentialLink(
            int emitterId,
            [MaybeNullWhen(false)] out Link link);
    }
}
