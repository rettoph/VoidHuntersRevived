using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Strategies;

namespace VoidHuntersRevived.Domain.Simulations.Common
{
    public interface IStepInput : IStepEvent
    {
        void Input(VhId sourceId, IStrategy strategy);
    }

    public interface IStepInput<TSelf> : IStepEvent<TSelf>, IStepInput
        where TSelf : class, IStepEvent<TSelf>
    {
        void IStepInput.Input(VhId sourceId, IStrategy strategy)
        {
            throw new NotImplementedException();
        }
    }
}