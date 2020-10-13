using Microsoft.Xna.Framework;
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

        public readonly Double Interval;

        public ActionTimer(Double interval)
        {
            this.Interval = interval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="action"></param>
        /// <param name="filter">Filter Action containing the current triggered state. True indicates that the last trigger has met or surpassed the interval rate.</param>
        public void Update(GameTime gameTime, Func<Boolean, Boolean> filter, Action<GameTime> action)
        {
            _lastTrigger += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (filter.Invoke(_lastTrigger >= this.Interval))
            {
                action.Invoke(gameTime);
                _lastTrigger = 0;
            }
        }
        public void Update(GameTime gameTime, Action<GameTime> action)
        {
            _lastTrigger += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_lastTrigger >= this.Interval)
            {
                action.Invoke(gameTime);
                _lastTrigger = 0;
            }
        }
    }
}
