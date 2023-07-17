using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Events;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Game.Enums;

namespace VoidHuntersRevived.Game.Events
{
    public class Helm_SetDirection : IInputData
    {
        public required VhId ShipVhId { get; init; }
        public required Direction Which { get; init; }
        public required bool Value { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<Helm_SetDirection, VhId, VhId, Direction, bool>.Instance.Calculate(source, this.ShipVhId, this.Which, this.Value);
        }
    }
}
