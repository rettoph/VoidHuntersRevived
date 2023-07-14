using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Serialization;

namespace VoidHuntersRevived.Game.Events
{
    internal sealed class TractorBeamEmitter_Activate : IEventData
    {
        public static readonly VhId NameSpace = new VhId("7FAE49C3-E794-494F-B5BC-E12238CC3020");

        public required VhId TractorBeamEmitterVhId { get; init; }
        public required EntityData TargetData { get; init; }
    }
}
