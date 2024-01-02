using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Core.Utilities;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Ships.Events
{
    internal sealed class TractorBeamEmitter_Select : IEventData
    {
        public bool IsPredictable => true;

        public required VhId TractorBeamEmitterVhId { get; init; }
        public required EntityData TargetData { get; init; }
        public required Location Location { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<TractorBeamEmitter_Select, VhId, VhId, VhId>.Instance.Calculate(source, this.TractorBeamEmitterVhId, this.TargetData.Id);
        }
    }
}
