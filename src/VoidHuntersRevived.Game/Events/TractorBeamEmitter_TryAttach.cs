using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Game.Events
{
    public class TractorBeamEmitter_TryAttach : IInputData
    {
        public required VhId ShipVhId { get; init; }
        public required VhId TargetVhId { get; init; }
        public required SocketVhId SocketVhId { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<TractorBeamEmitter_TryActivate, VhId, VhId, VhId, SocketVhId>.Instance.Calculate(source, this.ShipVhId, this.TargetVhId, this.SocketVhId);
        }
    }
}
