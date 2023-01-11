﻿using Guppy.Common;
using Guppy.Network.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Simulations.Events
{
    public class PlayerAction : ISimulationData
    {
        public required UserAction UserAction { get; init; }
    }
}
