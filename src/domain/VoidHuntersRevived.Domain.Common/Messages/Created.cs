using Guppy.Messaging;

namespace VoidHuntersRevived.Domain.Common.Messages
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
