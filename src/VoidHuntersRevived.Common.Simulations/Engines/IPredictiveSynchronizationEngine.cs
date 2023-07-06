using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Lockstep;

namespace VoidHuntersRevived.Common.Simulations.Engines
{
    [SimulationTypeFilter(SimulationType.Predictive)]
    public interface IPredictiveSynchronizationEngine : IEngine
    {
        void Initialize(ILockstepSimulation lockstep);

        void Synchronize(Step step);
    }
}
