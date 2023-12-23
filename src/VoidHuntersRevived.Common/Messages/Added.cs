using Guppy.Messaging;

namespace VoidHuntersRevived.Common.Messages
{
    public class Added<TItem, TContainer> : Message<Added<TItem, TContainer>>
    {
        public readonly TItem Item;
        public readonly TContainer Container;

        public Added(TItem item, TContainer container)
        {
            Item = item;
            Container = container;
        }
    }
}
