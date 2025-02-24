using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Tests.Common.Simulations.Mocks;
using Xunit;

namespace VoidHuntersRevived.Tests.Common.Physics.Extensions
{
    public static class StrategyMockerExtensions
    {
        public static IStrategyMocker AssertBodyCount(this IStrategyMocker strategy, int count)
        {
            Assert.Equal(count, strategy.Instance.Resolve<ISpace>().BodyCount);

            return strategy;
        }
    }
}