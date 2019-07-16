using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.CustomEventArgs
{
    public class ChangedEventArgs<TChanged> : EventArgs
    {
        public readonly TChanged Old;
        public readonly TChanged New;

        public ChangedEventArgs(TChanged oldValue, TChanged newValue)
        {
            this.Old = oldValue;
            this.New = newValue;
        }
    }
}
