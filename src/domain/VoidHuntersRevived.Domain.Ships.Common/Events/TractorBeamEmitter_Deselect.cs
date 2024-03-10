using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Ships.Common.Events
{
    internal sealed class TractorBeamEmitter_Deselect : IEventData
    {
        public bool IsPredictable => true;

        public required VhId TractorBeamEmitterVhId { get; init; }
        public required EntityData TargetData { get; init; }
        public required Location Location { get; init; }
        public required SocketVhId? AttachToSocketVhId { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<TractorBeamEmitter_Deselect, VhId, VhId, VhId, bool, SocketVhId>.Instance.Calculate(source, this.TractorBeamEmitterVhId, this.TargetData.Id, this.AttachToSocketVhId.HasValue, this.AttachToSocketVhId ?? default);
        }
    }
}
