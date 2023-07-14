using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Game.Events
{
    public class TractorBeamEmitter_TryDeactivate : IInputData
    {
        public static readonly VhId NameSpace = new VhId("2718961F-6E3E-4D1E-8EE2-63E73C152559");

        public required VhId ShipVhId { get; init; }
    }
}
