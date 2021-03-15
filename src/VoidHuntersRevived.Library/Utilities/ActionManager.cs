using Guppy.Events.Delegates;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Events;

namespace VoidHuntersRevived.Library.Utilities
{
    public sealed class ActionManager<TSender> : IDisposable
    {
        #region Private Fields
        private TSender _sender;
        private Dictionary<UInt32, OnGameTimeEventDelegate<TSender, Object[]>> _handlers;
        #endregion

        #region Constructors
        public ActionManager(TSender sender)
        {
            _sender = sender;
            _handlers = new Dictionary<UInt32, OnGameTimeEventDelegate<TSender, Object[]>>();
        }
        public void Dispose()
        {
            _handlers.Clear();

            _sender = default;
            _handlers = default;
        }
        #endregion

        #region API Methods
        public void TryInvoke(UInt32 handle, GameTime gameTime, params Object[] args)
        {
            if (_handlers.ContainsKey(handle))
                _handlers[handle]?.Invoke(_sender, gameTime, args);
        }

        public void Add(UInt32 handle, OnGameTimeEventDelegate<TSender, Object[]> handler)
        {
            if (_handlers.ContainsKey(handle))
                _handlers[handle] += handler;
            else
                _handlers[handle] = handler;
        }

        public void Remove(UInt32 handle, OnGameTimeEventDelegate<TSender, Object[]> handler)
        {
            if (!_handlers.ContainsKey(handle))
                return;
        }
        #endregion
    }
}
