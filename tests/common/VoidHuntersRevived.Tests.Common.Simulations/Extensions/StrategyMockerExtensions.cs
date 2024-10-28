using Svelto.ECS;
using Xunit;

namespace VoidHuntersRevived.Tests.Common.Simulations.Extensions
{
    public static class StrategyMockerExtensions
    {
        public static IStrategyMocker AssertEntityCount<T>(this IStrategyMocker strategy, int count)
            where T : unmanaged, IEntityComponent
        {
            Assert.Equal(count, strategy.CalculateTotalEntities<T>());

            return strategy;
        }
    }
}
