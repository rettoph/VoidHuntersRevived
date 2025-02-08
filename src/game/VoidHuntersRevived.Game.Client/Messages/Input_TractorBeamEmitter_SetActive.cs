using Guppy.Game.Input.Common;

namespace VoidHuntersRevived.Game.Client.Messages
{
    public sealed class Input_TractorBeamEmitter_SetActive(bool value) : InputMessage<Input_TractorBeamEmitter_SetActive>, IInputMessage
    {
        public readonly bool Value = value;
    }
}