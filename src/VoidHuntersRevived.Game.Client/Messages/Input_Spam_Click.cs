using Guppy.Game.Input;
using Guppy.Messaging;

namespace VoidHuntersRevived.Game.Client.Messages
{
    internal class Input_Spam_Click : Message<Input_Spam_Click>, IInput
    {
        public static Input_Spam_Click True = new Input_Spam_Click(true);
        public static Input_Spam_Click False = new Input_Spam_Click(false);

        public readonly bool Value;

        public Input_Spam_Click(bool value)
        {
            Value = value;
        }
    }
}
