using Guppy.Game.Input.Common;
using Guppy.Core.Messaging.Common;

namespace VoidHuntersRevived.Game.Client.Messages
{
    internal sealed class Input_TractorBeamEmitter_SetActive(bool value) : Message<Input_TractorBeamEmitter_SetActive>, IInput
    {
        public readonly bool Value = value;
    }
}
