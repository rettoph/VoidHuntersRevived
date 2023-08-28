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
    public class Input_TractorBeamEmitter_Deselect : IInputData
    {
        public required VhId ShipVhId { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<Input_TractorBeamEmitter_Deselect, VhId, VhId>.Instance.Calculate(source, this.ShipVhId);
        }
    }
}
