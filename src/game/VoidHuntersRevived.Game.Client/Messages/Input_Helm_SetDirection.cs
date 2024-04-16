using Guppy.Game.Input.Common;
using Guppy.Core.Messaging.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Enums;

namespace VoidHuntersRevived.Game.Core.Events
{
    public class Input_Helm_SetDirection : Message<Input_Helm_SetDirection>, IInput
    {
        public required Direction Which { get; init; }
        public required bool Value { get; init; }
    }
}
