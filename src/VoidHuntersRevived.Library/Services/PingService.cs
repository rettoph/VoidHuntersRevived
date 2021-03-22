using Guppy;
using Guppy.DependencyInjection;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Services
{
    /// <summary>
    /// Simple service useful for managing the creation of commonly used ping messages.
    /// "pings" can be enqueued and will be sent no matter what on an internally defined
    /// interval (probably near 150 milliseconds).
    /// </summary>
    public sealed class PingService : Frameable
    {
        #region Structs
        private struct Pinger
        {
            public Action action;
            public Boolean flag;
        }
        #endregion

        #region Private Fields
        private ActionTimer _actionTimer;
        private Pinger[] _pingerBuffer;
        private Int32 _pingerSize;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            _actionTimer = new ActionTimer(150);
            _pingerBuffer = new Pinger[250];
            _pingerSize = 0;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _actionTimer.Update(gameTime, this.Flush);
        }
        #endregion

        #region Helper Methods
        private void Flush(GameTime gameTime)
        {
            for(Int32 i=0; i<_pingerSize; i++)
            {
                _pingerBuffer[i].action();
                _pingerBuffer[i].flag = !_pingerBuffer[i].flag;
            }

            _pingerSize = 0;
        }

        public void Enqueue(Action action, ref Boolean flag)
        {
            _pingerBuffer[_pingerSize].action = action;
            _pingerBuffer[_pingerSize].flag = flag;
            _pingerSize++;

            if (_pingerSize >= _pingerBuffer.Length)
                this.Flush(default);
        }
        #endregion
    }
}
