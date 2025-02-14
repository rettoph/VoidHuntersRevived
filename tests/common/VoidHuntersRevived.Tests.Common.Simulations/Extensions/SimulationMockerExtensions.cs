using Svelto.ECS;
using VoidHuntersRevived.Tests.Common.Simulations.Mocks;

namespace VoidHuntersRevived.Tests.Common.Simulations.Extensions
{
    public static class SimulationMockerExtensions
    {
        public static SimulationAutoMock AssertEntityCount<T>(this SimulationAutoMock simulation, int count)
            where T : unmanaged, IEntityComponent
        {
            foreach (IStrategyAutoMock strategy in simulation.Strategies)
            {
                strategy.AssertEntityCount<T>(count);
            }

            return simulation;
        }
    }
}