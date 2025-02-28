using VoidHuntersRevived.Tests.Common.Entities.Interfaces;
using VoidHuntersRevived.Tests.Common.Simulations.Mockers;

namespace VoidHuntersRevived.Tests.Common.Entities.Mockers
{
    public abstract class EntitySimulationMocker<TSelf, TLockstepStrategyMocker, TPredictiveStrategyMocker> : SimulationMocker<TSelf, TLockstepStrategyMocker, TPredictiveStrategyMocker>, IEntitySimulationMocker<TSelf, TLockstepStrategyMocker, TPredictiveStrategyMocker>
        where TSelf : EntitySimulationMocker<TSelf, TLockstepStrategyMocker, TPredictiveStrategyMocker>
        where TLockstepStrategyMocker : EntityLockstepStrategyMocker, new()
        where TPredictiveStrategyMocker : EntityPredictiveStrategyMocker, new()
    {
        EntityLockstepStrategyMocker IEntitySimulationMocker.EntityLockstepStrategyMocker => this.LockstepStrategyMocker;
        EntityPredictiveStrategyMocker IEntitySimulationMocker.EntityPredictiveStrategyMocker => this.PredictiveStrategyMocker;
    }

    public sealed class EntitySimulationMocker<TLockstepStrategyMocker, TPredictiveStrategyMocker> : EntitySimulationMocker<EntitySimulationMocker<TLockstepStrategyMocker, TPredictiveStrategyMocker>, TLockstepStrategyMocker, TPredictiveStrategyMocker>
        where TLockstepStrategyMocker : EntityLockstepStrategyMocker, new()
        where TPredictiveStrategyMocker : EntityPredictiveStrategyMocker, new()
    {
    }
}
