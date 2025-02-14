using Svelto.ECS;
using VoidHuntersRevived.Tests.Common.Simulations.Mocks;
using Xunit;

namespace VoidHuntersRevived.Tests.Common.Simulations.Extensions
{
    public static class StrategyMockerExtensions
    {
        public static IStrategyAutoMock AssertEntityCount<T>(this IStrategyAutoMock strategy, int count)
            where T : unmanaged, IEntityComponent
        {
            Assert.Equal(count, strategy.CalculateTotalEntities<T>());

            return strategy;
        }
    }
}