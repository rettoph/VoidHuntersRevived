using Guppy.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Game.Common.Enums;

namespace VoidHuntersRevived.Game.Common.Events
{
    public class SetHelmDirectionInput : Message<SetHelmDirectionInput>
    {
        public required Direction Which { get; init; }
        public required bool Value { get; init; }
    }
}
