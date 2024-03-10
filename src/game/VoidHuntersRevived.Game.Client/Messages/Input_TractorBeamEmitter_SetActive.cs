using Guppy.Game.Input;
using Guppy.Messaging;

namespace VoidHuntersRevived.Game.Client.Messages
{
    internal sealed class Input_TractorBeamEmitter_SetActive : Message<Input_TractorBeamEmitter_SetActive>, IInput
    {
        public readonly bool Value;

        public Input_TractorBeamEmitter_SetActive(bool value)
        {
            Value = value;
        }
    }
}
