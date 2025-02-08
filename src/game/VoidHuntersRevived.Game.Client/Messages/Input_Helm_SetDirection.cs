using Guppy.Game.Input.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Enums;

namespace VoidHuntersRevived.Game.Core.Events
{
    public class Input_Helm_SetDirection : InputMessage<Input_Helm_SetDirection>, IInputMessage
    {
        public required DirectionEnum Which { get; init; }
        public required bool Value { get; init; }
    }
}