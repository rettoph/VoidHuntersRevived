using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Domain.Simulations.Messages
{
    internal class TickHistoryEnd
    {
        public required int CurrentTickId { get; init; }
    }
}
