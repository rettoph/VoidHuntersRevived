using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Game.Events
{
    public class TractorBeamEmitter_Deactivate : IEventData
    {
        public static readonly VhId NameSpace = new VhId("599A5204-6F73-400C-BA1D-64F89C8E2406");

        public required VhId TractorBeamEmitterVhId { get; init; }
        public required VhId TargetVhId { get; init; }
    }
}
