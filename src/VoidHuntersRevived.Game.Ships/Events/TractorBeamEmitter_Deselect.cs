﻿using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Game.Ships.Events
{
    internal sealed class TractorBeamEmitter_Deselect : IEventData
    {
        public required VhId TractorBeamEmitterVhId { get; init; }
        public required EntityData TargetData { get; init; }
        public required Location Location { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<TractorBeamEmitter_Deselect, VhId, VhId, VhId>.Instance.Calculate(source, this.TractorBeamEmitterVhId, this.TargetData.Id);
        }
    }
}
