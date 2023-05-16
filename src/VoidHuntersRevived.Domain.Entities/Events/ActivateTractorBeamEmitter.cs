using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Entities.Events
{
    public class ActivateTractorBeamEmitter : IMessage
    {
        public required ParallelKey TractorBeamEmitterKey { get; init; }

        public Type Type => typeof(ActivateTractorBeamEmitter);
    }
}
