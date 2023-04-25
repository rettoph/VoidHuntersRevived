using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations
{
    public class Input
    {
        public required Guid Id { get; init;  }
        public required ParallelKey Sender { get; init; }
    }
}
