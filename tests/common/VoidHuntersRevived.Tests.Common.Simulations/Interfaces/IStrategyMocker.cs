using Guppy.Tests.Common;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Strategies;

namespace VoidHuntersRevived.Tests.Common.Simulations.Interfaces
{
    public interface IStrategyMocker : IBuilder
    {
        void Update(TimeSpan interval, int count);

        void Input(VhId sourceId, IStepInput input);

        void Input(IStepInput input);

        void Input<TInput>()
            where TInput : IStepInput, new();
    }

    public interface IStrategyMocker<out TStrategy> : IStrategyMocker, IBuilder<TStrategy>
        where TStrategy : Strategy
    {
        TStrategy Strategy { get; }
    }
}
