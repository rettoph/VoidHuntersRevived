using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
