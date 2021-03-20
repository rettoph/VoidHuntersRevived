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
        private Dictionary<UInt32, ValidateGameTimeEventDelegate<TSender, Byte>> _handlers;
        #endregion

        #region Constructors
        public ActionManager(TSender sender)
        {
            _sender = sender;
            _handlers = new Dictionary<UInt32, ValidateGameTimeEventDelegate<TSender, Byte>>();
        }
        public void Dispose()
        {
            _handlers.Clear();

            _sender = default;
            _handlers = default;
        }
        #endregion

        #region API Methods
        /// <summary>
        /// By default, actions may be given a byte of data, usually reguarding state.
        /// This state value will be passed through the peer as needed.
        /// 
        /// Generally recoomended to use an Enum in this value. Extension methods
        /// should manage this accordingly
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="gameTime"></param>
        /// <param name="data"></param>
        /// <param name="force"></param>
        public Boolean TryInvoke(UInt32 handle, GameTime gameTime, Byte data = default, Boolean force = false)
        {
            if (_handlers.ContainsKey(handle))
                return _handlers[handle].Validate(_sender, gameTime, data, true);

            return true;
        }

        public void Add(UInt32 handle, ValidateGameTimeEventDelegate<TSender, Byte> handler)
        {
            if (_handlers.ContainsKey(handle))
                _handlers[handle] += handler;
            else
                _handlers[handle] = handler;
        }

        public void Remove(UInt32 handle, ValidateGameTimeEventDelegate<TSender, Byte> handler)
        {
            if (!_handlers.ContainsKey(handle))
                return;
        }
        #endregion
    }
}
