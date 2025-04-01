using VoidHuntersRevived.Tests.Common.Entities.Interfaces;
using VoidHuntersRevived.Tests.Common.Simulations.Mockers;

namespace VoidHuntersRevived.Tests.Common.Entities.Mockers
{
    public abstract class EntitySimulationMocker<TSelf, TStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker> : SimulationMocker<TSelf, TStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker>, IEntitySimulationMocker<TSelf, TStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker>
        where TSelf : EntitySimulationMocker<TSelf, TStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker>
        where TStrategyMocker : IEntityStrategyMocker
        where TLockstepStrategyMocker : EntityLockstepStrategyMocker, TStrategyMocker, new()
        where TPredictiveStrategyMocker : EntityPredictiveStrategyMocker, TStrategyMocker, new()
    {
        EntityLockstepStrategyMocker IEntitySimulationMocker.EntityLockstepStrategyMocker => this.LockstepStrategyMocker;
        EntityPredictiveStrategyMocker IEntitySimulationMocker.EntityPredictiveStrategyMocker => this.PredictiveStrategyMocker;
    }

    public sealed class EntitySimulationMocker<TLockstepStrategyMocker, TPredictiveStrategyMocker> : EntitySimulationMocker<EntitySimulationMocker<TLockstepStrategyMocker, TPredictiveStrategyMocker>, IEntityStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker>
        where TLockstepStrategyMocker : EntityLockstepStrategyMocker, IEntityStrategyMocker, new()
        where TPredictiveStrategyMocker : EntityPredictiveStrategyMocker, IEntityStrategyMocker, new()
    {
    }
}
