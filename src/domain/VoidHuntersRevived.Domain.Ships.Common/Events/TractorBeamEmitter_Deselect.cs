using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Ships.Common.Events
{
    public sealed class TractorBeamEmitter_Deselect : IEventData
    {
        public bool IsPredictable => true;

        public required EntityGlobalId TractorBeamEmitterGlobalId { get; init; }
        public required EntityData TargetData { get; init; }
        public required FixTransform2D Transform { get; init; }
        public required NodeSocketGlobalId? AttachToSocketVhId { get; init; }

        public VhId CalculateHash(in VhId source) => HashBuilder<TractorBeamEmitter_Deselect, VhId, EntityGlobalId, VhId, bool, NodeSocketGlobalId>.Instance.Calculate(source, this.TractorBeamEmitterGlobalId, this.TargetData.Id, this.AttachToSocketVhId.HasValue, this.AttachToSocketVhId ?? default);
    }
}