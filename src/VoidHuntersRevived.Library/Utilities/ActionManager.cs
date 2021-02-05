using Guppy.Events.Delegates;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Utilities
{
    public sealed class ActionManager<TSender> : IDisposable
    {
        #region Private Fields
        private TSender _sender;
        private Dictionary<UInt32, OnEventDelegate<TSender, Object>> _handlers;
        #endregion

        #region Constructors
        public ActionManager(TSender sender)
        {
            _sender = sender;
            _handlers = new Dictionary<UInt32, OnEventDelegate<TSender, Object>>();
        }
        public void Dispose()
        {
            _handlers.Clear();

            _sender = default;
            _handlers = default;
        }
        #endregion

        #region API Methods
        public void TryInvoke(UInt32 handle, Object args = null)
        {
            if (_handlers.ContainsKey(handle))
                _handlers[handle]?.Invoke(_sender, args);
        }

        public void Add(UInt32 handle, OnEventDelegate<TSender, Object> handler)
        {
            if (_handlers.ContainsKey(handle))
                _handlers[handle] += handler;
            else
                _handlers[handle] = handler;
        }

        public void Remove(UInt32 handle, OnEventDelegate<TSender, Object> handler)
        {
            if (!_handlers.ContainsKey(handle))
                return;
        }
        #endregion
    }
}
