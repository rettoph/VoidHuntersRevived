﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Events;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Game.Ships.Events
{
    public class Input_TractorBeamEmitter_Select : IInputData
    {
        public bool IsPredictable => true;

        public required VhId ShipVhId { get; init; }
        public required VhId TargetVhId { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<Input_TractorBeamEmitter_Select, VhId, VhId, VhId>.Instance.Calculate(source, this.ShipVhId, this.TargetVhId);
        }
    }
}
