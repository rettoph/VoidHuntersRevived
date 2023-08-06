using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Game.Ships.Events
{
    internal class TractorBeamEmitter_Detach
    {
        public required VhId TractorBeamEmitterVhId { get; init; }
        public required EntityData TargetData { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<TractorBeamEmitter_Activate, VhId, VhId, VhId>.Instance.Calculate(source, this.TractorBeamEmitterVhId, this.TargetData.Id);
        }
    }
}
