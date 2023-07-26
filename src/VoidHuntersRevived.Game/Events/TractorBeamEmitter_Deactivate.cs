using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Simulations.Events;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Game.Events
{
    public class TractorBeamEmitter_Deactivate : IEventData
    {
        public required VhId TractorBeamEmitterVhId { get; init; }
        public required VhId TargetVhId { get; init; }
        public required SocketVhId? AttachTo { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<TractorBeamEmitter_Deactivate, VhId, VhId, VhId, SocketVhId>.Instance.Calculate(source, this.TractorBeamEmitterVhId, this.TargetVhId, this.AttachTo ?? default);
        }
    }
}
