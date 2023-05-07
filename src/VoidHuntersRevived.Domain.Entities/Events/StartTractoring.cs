﻿using Guppy.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Entities.Events
{
    public sealed class StartTractoring : IMessage
    {
        public required ParallelKey TractorBeamEmitterKey { get; init; }

        public Type Type => typeof(StartTractoring);
    }
}
