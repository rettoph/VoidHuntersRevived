using Guppy.Core.Messaging.Common;
using Guppy.Game.Input.Common;

namespace VoidHuntersRevived.Game.Client.Messages
{
    internal class Input_Spam_Click(bool value) : Message<Input_Spam_Click>, IInput
    {
        public static Input_Spam_Click True = new(true);
        public static Input_Spam_Click False = new(false);

        public readonly bool Value = value;
    }
}