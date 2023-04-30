using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Simulations
{
    public static class ParallelKeys
    {
        public static ParallelKey System = ParallelEntityTypes.Pilot.Create(int.MaxValue);
    }
}
