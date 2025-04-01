using VoidHuntersRevived.Tests.Common.Entities.Mockers;
using VoidHuntersRevived.Tests.Common.Simulations.Interfaces;

namespace VoidHuntersRevived.Tests.Common.Entities.Interfaces
{
    public interface IEntitySimulationMocker : ISimulationMocker
    {
        EntityLockstepStrategyMocker EntityLockstepStrategyMocker { get; }
        EntityPredictiveStrategyMocker EntityPredictiveStrategyMocker { get; }
    }
    public interface IEntitySimulationMocker<out TSelf, TStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker> : ISimulationMocker<TSelf, TStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker>, IEntitySimulationMocker
        where TSelf : IEntitySimulationMocker<TSelf, TStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker>
        where TStrategyMocker : IEntityStrategyMocker
        where TLockstepStrategyMocker : EntityLockstepStrategyMocker, TStrategyMocker, new()
        where TPredictiveStrategyMocker : EntityPredictiveStrategyMocker, TStrategyMocker, new()
    {
    }
}
