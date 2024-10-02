using Guppy.Game.Input.Common;
using Guppy.Core.Messaging.Common;

namespace VoidHuntersRevived.Game.Client.Messages
{
    internal class Input_Spam_Click(bool value) : Message<Input_Spam_Click>, IInput
    {
        public static Input_Spam_Click True = new Input_Spam_Click(true);
        public static Input_Spam_Click False = new Input_Spam_Click(false);

        public readonly bool Value = value;
    }
}
