using Svelto.ECS;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Lockstep;

namespace VoidHuntersRevived.Common.Simulations.Engines
{
    [SimulationFilter(SimulationType.Predictive)]
    public interface IPredictiveSynchronizationEngine : IEngine
    {
        void Initialize(ILockstepSimulation lockstep);

        void Synchronize(Step step);
    }
}
