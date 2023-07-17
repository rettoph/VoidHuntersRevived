using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations.Events;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Game.Events
{
    public class TractorBeamEmitter_TryDeactivate : IInputData
    {
        public required VhId ShipVhId { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<TractorBeamEmitter_TryDeactivate, VhId, VhId>.Instance.Calculate(source, this.ShipVhId);
        }
    }
}
