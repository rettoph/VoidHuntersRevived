using Guppy.Messaging;

namespace VoidHuntersRevived.Common.Messages
{
    public class Destroyed<T> : Message<Destroyed<T>>
    {
        public readonly T Instance;

        public Destroyed(T instance)
        {
            Instance = instance;
        }
    }
}
