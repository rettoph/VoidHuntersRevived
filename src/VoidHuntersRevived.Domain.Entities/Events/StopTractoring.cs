﻿using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Entities.Events
{
    public class StopTractoring : Message<StopTractoring>, IData
    {
        public static readonly StopTractoring Instance = new StopTractoring();
    }
}
