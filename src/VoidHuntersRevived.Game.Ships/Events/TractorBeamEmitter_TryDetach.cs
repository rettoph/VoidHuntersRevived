using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Game.Ships.Events
{
    public class TractorBeamEmitter_TryDetach : IInputData
    {
        public required VhId ShipVhId { get; init; }
        public required VhId TargetVhId { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<TractorBeamEmitter_TryActivate, VhId, VhId, VhId>.Instance.Calculate(source, this.ShipVhId, this.TargetVhId);
        }
    }
}
