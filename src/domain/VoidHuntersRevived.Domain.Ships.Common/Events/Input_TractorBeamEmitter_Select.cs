using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Ships.Common.Events
{
    public class Input_TractorBeamEmitter_Select : IStepInput<Input_TractorBeamEmitter_Select>
    {
        public bool IsPredictable => true;

        public required EntityGlobalId TractorBeamEmitterGlobalId { get; init; }
        public required EntityGlobalId TargetNodeGlobalId { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<Input_TractorBeamEmitter_Select, VhId, EntityGlobalId, EntityGlobalId>.Instance.Calculate(source, this.TractorBeamEmitterGlobalId, this.TargetNodeGlobalId);
        }
    }
}