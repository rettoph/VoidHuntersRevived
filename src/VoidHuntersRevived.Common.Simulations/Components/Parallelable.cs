using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations.Components
{
    public sealed class Parallelable
    {
        public readonly ParallelKey Key;

        public Parallelable(ParallelKey key)
        {
            this.Key = key;
        }
    }
}
