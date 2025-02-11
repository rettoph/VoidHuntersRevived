using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Ships.Common.Events
{
    public sealed class TractorBeamEmitter_Select : IStepEvent<TractorBeamEmitter_Select>
    {
        public bool IsPredictable => true;

        public required EntityGlobalId TractorBeamEmitterGlobalId { get; init; }
        public required EntityData TargetData { get; init; }
        public required FixTransform2D Transform { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<TractorBeamEmitter_Select, VhId, EntityGlobalId, VhId>.Instance.Calculate(source, this.TractorBeamEmitterGlobalId, this.TargetData.Id);
        }
    }
}