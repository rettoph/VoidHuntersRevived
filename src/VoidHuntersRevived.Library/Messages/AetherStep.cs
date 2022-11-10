using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Messages
{
    public class AetherStep : Message
    {
        public readonly TimeSpan Interval;

        public AetherStep(TimeSpan interval)
        {
            this.Interval = interval;
        }
    }
}
