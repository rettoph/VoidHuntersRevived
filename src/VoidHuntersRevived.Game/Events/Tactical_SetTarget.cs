﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Game.Events
{
    public class Tactical_SetTarget : IInputData
    {
        public static readonly VhId NameSpace = new VhId("C942030A-8541-44E3-8658-B3F3AB1FD0D7");

        public required VhId ShipVhId { get; init; }
        public required FixVector2 Value { get; init; }
    }
}
