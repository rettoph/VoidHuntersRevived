using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Pieces.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Ships.Common.Events
{
    public class Helm_SetDirection : IInputData
    {
        public bool IsPredictable => true;

        public required VhId ShipVhId { get; init; }
        public required Direction Which { get; init; }
        public required bool Value { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<Helm_SetDirection, VhId, VhId, Direction, bool>.Instance.Calculate(source, this.ShipVhId, this.Which, this.Value);
        }
    }
}
