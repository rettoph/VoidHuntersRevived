using VoidHuntersRevived.Tests.Common.Simulations.Mocks;

namespace VoidHuntersRevived.Tests.Common.Physics.Extensions
{
    public static class SimulationMockerExtensions
    {
        public static SimulationMock AssertBodyCount(this SimulationMock simulation, int count)
        {
            foreach (IStrategyAutoMock strategy in simulation.Strategies)
            {
                strategy.AssertBodyCount(count);
            }

            return simulation;
        }
    }
}