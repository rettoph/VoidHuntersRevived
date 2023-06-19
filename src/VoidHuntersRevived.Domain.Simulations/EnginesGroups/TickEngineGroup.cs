using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Entities.EnginesGroups;

namespace VoidHuntersRevived.Domain.Simulations.EnginesGroups
{
    public class TickEngineGroup : BulkEnginesGroups<ITickEngine, Tick>
    {
        public TickEngineGroup(IEnumerable<ITickEngine> engines) : base(engines)
        {
        }
    }
}
