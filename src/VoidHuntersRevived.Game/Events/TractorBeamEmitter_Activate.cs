using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Game.Events
{
    public class TractorBeamEmitter_Activate : IInputData
    {
        public required VhId ShipVhId { get; init; }
        public required VhId TargetVhId { get; init; }
    }
}
