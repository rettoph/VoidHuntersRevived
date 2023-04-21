using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations.Lockstep
{
    public sealed class EventDto
    {
        public readonly ParallelKey Sender;
        public readonly IData Data;

        public EventDto(ParallelKey sender, IData data)
        {
            this.Sender = sender;
            this.Data = data;
        }
    }
}
