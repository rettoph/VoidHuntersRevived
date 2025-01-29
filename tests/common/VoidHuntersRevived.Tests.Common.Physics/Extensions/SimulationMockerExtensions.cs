using Autofac;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Tests.Common.Simulations;
using Xunit;

namespace VoidHuntersRevived.Tests.Common.Physics.Extensions
{
    public static class StrategyMockerExtensions
    {
        public static IStrategyMocker AssertBodyCount(this IStrategyMocker strategy, int count)
        {
            Assert.Equal(count, strategy.Scope.ResolveService<ISpace>().BodyCount);

            return strategy;
        }
    }
}