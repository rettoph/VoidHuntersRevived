using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Game.Enums;

namespace VoidHuntersRevived.Game.Events
{
    public class SetHelmDirection : IInputData
    {
        public required VhId ShipId { get; init; }
        public required Direction Which { get; init; }
        public required bool Value { get; init; }
    }
}
