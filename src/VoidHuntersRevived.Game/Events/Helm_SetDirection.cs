using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Game.Enums;

namespace VoidHuntersRevived.Game.Events
{
    public class Helm_SetDirection : IInputData
    {
        public required VhId ShipVhId { get; init; }
        public required Direction Which { get; init; }
        public required bool Value { get; init; }
    }
}
