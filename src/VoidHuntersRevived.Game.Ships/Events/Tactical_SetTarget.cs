using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Game.Ships.Events
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
