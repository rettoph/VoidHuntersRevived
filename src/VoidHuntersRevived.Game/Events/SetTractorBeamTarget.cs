using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Events;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Game.Events
{
    public class SetTractorBeamTarget : IEventData
    {
        public required VhId TractorBeamId { get; init; }
        public required EntityData TargetData { get; init; }
    }
}
