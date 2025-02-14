using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Tests.Common.Simulations.Mocks;
using Xunit;

namespace VoidHuntersRevived.Tests.Common.Physics.Extensions
{
    public static class StrategyMockerExtensions
    {
        public static IStrategyAutoMock AssertBodyCount(this IStrategyAutoMock strategy, int count)
        {
            Assert.Equal(count, strategy.Instance.Resolve<ISpace>().BodyCount);

            return strategy;
        }
    }
}