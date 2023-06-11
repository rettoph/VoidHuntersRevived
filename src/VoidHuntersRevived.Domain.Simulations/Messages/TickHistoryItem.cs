using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Simulations.Messages
{
    internal class TickHistoryItem
    {
        public required Tick Tick { get; init; }
    }
}
