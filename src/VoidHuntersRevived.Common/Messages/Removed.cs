using Guppy.Messaging;

namespace VoidHuntersRevived.Common.Messages
{
    public class Removed<TItem, TContainer> : Message<Removed<TItem, TContainer>>
    {
        public readonly TItem Item;
        public readonly TContainer Container;

        public Removed(TItem item, TContainer container)
        {
            Item = item;
            Container = container;
        }
    }
}
