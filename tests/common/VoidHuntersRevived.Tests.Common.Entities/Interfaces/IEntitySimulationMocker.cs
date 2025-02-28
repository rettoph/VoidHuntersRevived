using VoidHuntersRevived.Tests.Common.Entities.Mockers;
using VoidHuntersRevived.Tests.Common.Simulations.Interfaces;

namespace VoidHuntersRevived.Tests.Common.Entities.Interfaces
{
    public interface IEntitySimulationMocker : ISimulationMocker
    {
        EntityLockstepStrategyMocker EntityLockstepStrategyMocker { get; }
        EntityPredictiveStrategyMocker EntityPredictiveStrategyMocker { get; }
    }
    public interface IEntitySimulationMocker<out TSelf, TLockstepStrategyMocker, TPredictiveStrategyMocker> : ISimulationMocker<TSelf, TLockstepStrategyMocker, TPredictiveStrategyMocker>, IEntitySimulationMocker
        where TSelf : IEntitySimulationMocker<TSelf, TLockstepStrategyMocker, TPredictiveStrategyMocker>
        where TLockstepStrategyMocker : EntityLockstepStrategyMocker, new()
        where TPredictiveStrategyMocker : EntityPredictiveStrategyMocker, new()
    {
    }
}
