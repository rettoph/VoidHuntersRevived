using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Ships.Common.Events
{
    public class Tactical_SetTarget : IInputData
    {
        public bool IsPredictable => true;

        public required VhId ShipVhId { get; init; }
        public required FixVector2 Value { get; init; }
        public bool Snap { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<Tactical_SetTarget, VhId, VhId, FixVector2>.Instance.Calculate(source, this.ShipVhId, this.Value);
        }
    }
}
