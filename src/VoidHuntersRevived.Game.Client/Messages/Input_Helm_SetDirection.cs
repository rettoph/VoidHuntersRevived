using Guppy.Game.Input;
using Guppy.Messaging;
using VoidHuntersRevived.Common.Pieces.Enums;

namespace VoidHuntersRevived.Game.Core.Events
{
    public class Input_Helm_SetDirection : Message<Input_Helm_SetDirection>, IInput
    {
        public required Direction Which { get; init; }
        public required bool Value { get; init; }
    }
}
