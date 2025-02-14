using VoidHuntersRevived.Domain.Simulations;

namespace VoidHuntersRevived.Tests.Common.Simulations.Interfaces
{
    public interface IBaseStrategyMocker
    {
        Strategy Strategy { get; }

        void Update(TimeSpan interval, int count);
    }

    public interface IBaseStrategyMocker<out TStrategy> : IBaseStrategyMocker
        where TStrategy : Strategy
    {
        new TStrategy Strategy { get; }
    }
}
