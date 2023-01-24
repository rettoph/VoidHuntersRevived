﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Events
{
    public class CreateNode : IData
    {
        public required int TreeId { get; init; }
        public required int NodeId { get; init; }
    }
}
