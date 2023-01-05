using Guppy.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations.Lockstep
{
    public class Step : GameTime, IMessage
    {
        public Type Type { get; } = typeof(Step);

        public Step(TimeSpan interval)
        {
            ElapsedGameTime = interval;
            TotalGameTime = TimeSpan.Zero;
        }

        internal Step Increment()
        {
            TotalGameTime += ElapsedGameTime;

            return this;
        }
    }
}
