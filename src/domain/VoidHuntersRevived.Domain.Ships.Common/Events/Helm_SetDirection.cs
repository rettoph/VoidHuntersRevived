using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Ships.Common.Events
{
    public class Helm_SetDirection : IStepInput<Helm_SetDirection>
    {
        public bool IsPredictable => true;

        public required EntityGlobalId ShipGlobalId { get; init; }
        public required DirectionEnum Which { get; init; }
        public required bool Value { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<Helm_SetDirection, VhId, EntityGlobalId, DirectionEnum, bool>.Instance.Calculate(source, this.ShipGlobalId, this.Which, this.Value);
        }
    }
}