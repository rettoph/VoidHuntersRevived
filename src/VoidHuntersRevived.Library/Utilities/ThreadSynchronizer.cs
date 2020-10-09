using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoidHuntersRevived.Library.Utilities
{
    public sealed class ThreadSynchronizer
    {
        private Queue<Action<GameTime>> _queue;

        public ThreadSynchronizer()
        {
            _queue = new Queue<Action<GameTime>>();

        }
        public void Update(GameTime gameTime)
        {
            while (_queue.Any())
                _queue.Dequeue()(gameTime);
        }

        public void Do(Action<GameTime> action)
            => _queue.Enqueue(action);
    }
}
