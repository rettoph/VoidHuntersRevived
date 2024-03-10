using Guppy.Messaging;

namespace VoidHuntersRevived.Common.Messages
{
    public class Created<T> : Message<Created<T>>
    {
        public readonly T Instance;

        public Created(T instance)
        {
            Instance = instance;
        }
    }
}
