using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Ships.Common.Events
{
    public class Tactical_SetTarget : IInputData
    {
        public bool IsPredictable => true;

        public required EntityGlobalId ShipGlobalId { get; init; }
        public required FixVector2 Value { get; init; }
        public bool Snap { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<Tactical_SetTarget, VhId, EntityGlobalId, FixVector2>.Instance.Calculate(source, this.ShipGlobalId, this.Value);
        }
    }
}