using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Entities.Events
{
    public class SetTractorBeamEmitterTarget
    {
        public required EventId TractorBeamEmitterKey { get; init; }
        public required EventId TargetKey { get; init; }
    }
}
