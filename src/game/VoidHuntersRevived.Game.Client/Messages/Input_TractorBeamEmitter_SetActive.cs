using Guppy.Core.Messaging.Common;
using Guppy.Game.Input.Common;

namespace VoidHuntersRevived.Game.Client.Messages
{
    internal sealed class Input_TractorBeamEmitter_SetActive(bool value) : Message<Input_TractorBeamEmitter_SetActive>, IInput
    {
        public readonly bool Value = value;
    }
}