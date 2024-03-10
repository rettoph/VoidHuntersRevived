using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Common.Engines
{
    [SimulationFilter(SimulationType.Predictive)]
    public interface IPredictiveSynchronizationEngine : IEngine
    {
        void Initialize(ILockstepSimulation lockstep);

        void Synchronize(Step step);
    }
}
