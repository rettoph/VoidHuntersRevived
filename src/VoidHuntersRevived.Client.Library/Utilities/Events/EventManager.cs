using Guppy;
using Guppy.Extensions.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Windows.Library.Utilities.Events
{
    public class EventManager<TSender, TArgs>
    {
        public delegate Boolean ValidateEventDelegate(TSender sender, TArgs args);
        public delegate void OnEventDelegate(TSender sender, TArgs args);

        public event ValidateEventDelegate ValidateEvent;
        public event OnEventDelegate OnEvent;

        public readonly TSender Sender;

        public EventManager(TSender sender)
        {
            this.Sender = sender;
        }

        public Boolean TryInvokeEvent(TArgs args)
        {
            if (!this.ValidateEvent?.Invoke(this.Sender, args) ?? true)
                return false;

            this.OnEvent?.Invoke(this.Sender, args);
            return true;
        }
    }
}
