using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Game.Events
{
    internal class TractorBeamEmitter_Attach : IEventData
    {
        public required VhId TractorBeamEmitterVhId { get; init; }
        public required VhId TargetVhId { get; init; }
        public required SocketVhId SocketVhId { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<TractorBeamEmitter_Deactivate, VhId, VhId, VhId, SocketVhId>.Instance.Calculate(source, this.TractorBeamEmitterVhId, this.TargetVhId, this.SocketVhId);
        }
    }
}
