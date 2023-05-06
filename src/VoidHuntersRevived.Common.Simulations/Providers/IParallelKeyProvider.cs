using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations.Providers
{
    public interface IParallelKeyProvider
    {
        ParallelKey Next(in ParallelKey key);

        ParallelKey Previous(in ParallelKey key);
    }
}
