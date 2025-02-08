using Guppy.Game.Input.Common;

namespace VoidHuntersRevived.Game.Client.Messages
{
    public class Input_Spam_Click(bool value) : InputMessage<Input_Spam_Click>, IInputMessage
    {
        public static Input_Spam_Click True = new(true);
        public static Input_Spam_Click False = new(false);

        public readonly bool Value = value;
    }
}