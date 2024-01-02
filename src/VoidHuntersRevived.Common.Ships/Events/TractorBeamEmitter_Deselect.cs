using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Core.Utilities;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Ships.Events
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
