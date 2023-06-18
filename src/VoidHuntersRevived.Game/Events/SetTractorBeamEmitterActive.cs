using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Game.Events
{
    public class SetTractorBeamEmitterActive : IInputData
    {
        public required VhId ShipId { get; init; }
        public required bool Value { get; init; }
    }
}
