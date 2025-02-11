using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Ships.Common.Events
{
    public class Input_TractorBeamEmitter_Deselect : IStepInput<Input_TractorBeamEmitter_Deselect>
    {
        public bool IsPredictable => true;

        public required EntityGlobalId TractorBeamEmitterGlobalId { get; init; }
        public required NodeSocketGlobalId? AttachToNodeSocketGlobalId { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<Input_TractorBeamEmitter_Deselect, VhId, EntityGlobalId, bool, NodeSocketGlobalId>.Instance.Calculate(source, this.TractorBeamEmitterGlobalId, this.AttachToNodeSocketGlobalId.HasValue, this.AttachToNodeSocketGlobalId ?? default);
        }
    }
}