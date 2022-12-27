using Guppy.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library
{
    public class Step : GameTime, IMessage
    {
        public Type Type { get; } = typeof(Step);

        public Step(TimeSpan interval)
        {
            this.ElapsedGameTime = interval;
            this.TotalGameTime = TimeSpan.Zero;
        }

        internal Step Increment()
        {
            this.TotalGameTime += this.ElapsedGameTime;

            return this;
        }
    }
}
