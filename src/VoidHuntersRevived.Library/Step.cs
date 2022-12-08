using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library
{
    public class Step : Message, IMessage
    {
        public readonly TimeSpan Interval;

        public Step(TimeSpan interval)
        {
            Interval = interval;
        }
    }
}
