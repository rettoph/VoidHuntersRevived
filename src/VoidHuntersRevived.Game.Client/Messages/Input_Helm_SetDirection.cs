using Guppy.Common;
using VoidHuntersRevived.Common.Pieces.Enums;

namespace VoidHuntersRevived.Game.Common.Events
{
    public class Input_Helm_SetDirection : Message<Input_Helm_SetDirection>
    {
        public required Direction Which { get; init; }
        public required bool Value { get; init; }
    }
}
