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
            this.ElapsedGameTime = interval;
            this.TotalGameTime = interval;
        }

        private Step(Step previous)
        {
            this.ElapsedGameTime = previous.ElapsedGameTime;
            this.TotalGameTime = previous.TotalGameTime + this.ElapsedGameTime;
        }

        internal Step Next()
        {
            return new Step(this);
        }
    }
}
