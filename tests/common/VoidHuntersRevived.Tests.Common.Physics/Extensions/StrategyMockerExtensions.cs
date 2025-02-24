using VoidHuntersRevived.Tests.Common.Simulations.Mocks;

namespace VoidHuntersRevived.Tests.Common.Physics.Extensions
{
    public static class SimulationMockerExtensions
    {
        public static SimulationMockerOld AssertBodyCount(this SimulationMockerOld simulation, int count)
        {
            foreach (IStrategyMocker strategy in simulation.Strategies)
            {
                strategy.AssertBodyCount(count);
            }

            return simulation;
        }
    }
}