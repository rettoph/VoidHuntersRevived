using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Entities.Events
{
    public class CreateTractorBeamEmitterTarget
    {
        public required ParallelKey TractorBeamEmitterKey { get; init; }
        public required ShipPart ShipPart { get; init; }
    }
}
