using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Utilities
{
    /// <summary>
    /// Utility automatically updated each frame
    /// via a network scene. This allows resources
    /// to query if the current frame ended or lapped
    /// an interval of a specific amount of milliseconds.
    /// 
    /// Used promarily internally for bullet firing, message
    /// intervals, and more.
    /// </summary>
    public class Interval
    {
        private Double _interval;
        private Double _totalGameTime;
        private ConcurrentDictionary<Double, Double> _intervals;

        public Interval()
        {
            _intervals = new ConcurrentDictionary<Double, Double>();
        }

        internal void Update(GameTime gameTime)
        {
            _totalGameTime = gameTime.TotalGameTime.TotalMilliseconds;

            foreach(KeyValuePair<Double, Double> interval in _intervals)
                _intervals[interval.Key] = (_intervals[interval.Key] % interval.Key) + gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        public Boolean Is(Double milliseconds)
        {
            if(!_intervals.TryGetValue(milliseconds, out _interval))
            {
                _intervals.TryAdd(milliseconds, _totalGameTime % milliseconds);
                _interval = _intervals[milliseconds];
            }

            return _interval >= milliseconds;
        }
    }
}
