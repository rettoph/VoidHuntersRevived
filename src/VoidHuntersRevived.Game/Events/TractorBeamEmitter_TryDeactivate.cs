using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations.Events;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Common.Pieces;

namespace VoidHuntersRevived.Game.Events
{
    public class TractorBeamEmitter_TryDeactivate : IInputData
    {
        public required VhId ShipVhId { get; init; }
        public required JointVhId? AttachTo { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<TractorBeamEmitter_TryDeactivate, VhId, VhId, JointVhId>.Instance.Calculate(source, this.ShipVhId, this.AttachTo ?? default);
        }
    }
}
