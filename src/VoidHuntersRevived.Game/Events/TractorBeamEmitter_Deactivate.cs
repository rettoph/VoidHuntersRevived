using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Game.Events
{
    public class TractorBeamEmitter_Deactivate : IInputData
    {
        public required VhId ShipVhId { get; init; }
    }
}
