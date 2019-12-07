﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Utilities
{
    /// <summary>
    /// Simple class used to trigger an action every N 
    /// milliseconds when Updated
    /// </summary>
    public sealed class ActionTimer
    {
        private Double _lastTrigger;
        private Double _interval;

        public ActionTimer(Double interval)
        {
            _interval = interval;
        }


        public void Update(GameTime gameTime, Action action, Func<Boolean> filter)
        {
            _lastTrigger += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_lastTrigger >= _interval && filter.Invoke())
            {
                action.Invoke();
                _lastTrigger = 0;
            }
        }
        public void Update(GameTime gameTime, Action action)
        {
            _lastTrigger += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_lastTrigger >= _interval)
            {
                action.Invoke();
                _lastTrigger = 0;
            }
        }
    }
}
