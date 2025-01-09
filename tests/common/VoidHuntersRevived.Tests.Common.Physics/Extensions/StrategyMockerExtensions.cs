using VoidHuntersRevived.Tests.Common.Simulations;

namespace VoidHuntersRevived.Tests.Common.Physics.Extensions
{
    public static class SimulationMockerExtensions
    {
        public static SimulationMocker AssertBodyCount(this SimulationMocker simulation, int count)
        {
            foreach (IStrategyMocker strategy in simulation.Strategies)
            {
                strategy.AssertBodyCount(count);
            }

            return simulation;
        }
    }
}