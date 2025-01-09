using Svelto.ECS;

namespace VoidHuntersRevived.Tests.Common.Simulations.Extensions
{
    public static class SimulationMockerExtensions
    {
        public static SimulationMocker AssertEntityCount<T>(this SimulationMocker simulation, int count)
            where T : unmanaged, IEntityComponent
        {
            foreach (IStrategyMocker strategy in simulation.Strategies)
            {
                strategy.AssertEntityCount<T>(count);
            }

            return simulation;
        }
    }
}