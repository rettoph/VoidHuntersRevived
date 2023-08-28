using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Events;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Game.Ships.Events
{
    internal sealed class TractorBeamEmitter_Select : IEventData
    {
        public required VhId TractorBeamEmitterVhId { get; init; }
        public required EntityData TargetData { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<TractorBeamEmitter_Select, VhId, VhId, VhId>.Instance.Calculate(source, this.TractorBeamEmitterVhId, this.TargetData.Id);
        }
    }
}
