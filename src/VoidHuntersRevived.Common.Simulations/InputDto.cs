using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations
{
    public class InputDto
    {
        public Guid Id { get; init; }
        public ParallelKey Sender { get; init; }
        public IData Data { get; init; }

        public InputDto()
        {
        }
    }
}
