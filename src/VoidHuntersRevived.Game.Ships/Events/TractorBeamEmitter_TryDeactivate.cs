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
using VoidHuntersRevived.Common.Simulations.Attributes;

namespace VoidHuntersRevived.Game.Ships.Events
{
    [Predictable]
    public class TractorBeamEmitter_TryDeactivate : IInputData
    {
        public required VhId ShipVhId { get; init; }
        public required VhId TargetVhId { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<TractorBeamEmitter_TryDeactivate, VhId, VhId, VhId>.Instance.Calculate(source, this.ShipVhId, this.TargetVhId);
        }
    }
}
