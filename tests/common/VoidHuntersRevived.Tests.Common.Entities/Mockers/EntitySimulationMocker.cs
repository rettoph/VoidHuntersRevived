using VoidHuntersRevived.Tests.Common.Entities.Interfaces;

namespace VoidHuntersRevived.Tests.Common.Entities.Mockers
{
    public class EntitySimulationMocker<TLockstepStrategyMocker, TPredictiveStrategyMocker> : SimulationMocker<TLockstepStrategyMocker, TPredictiveStrategyMocker>, IEntitySimulationMocker<TLockstepStrategyMocker, TPredictiveStrategyMocker>
        where TLockstepStrategyMocker : EntityLockstepStrategyMocker, new()
        where TPredictiveStrategyMocker : EntityPredictiveStrategyMocker, new()
    {
        EntityLockstepStrategyMocker IEntitySimulationMocker.EntityLockstepStrategyMocker => this.LockstepStrategyMocker;
        EntityPredictiveStrategyMocker IEntitySimulationMocker.EntityPredictiveStrategyMocker => this.PredictiveStrategyMocker;
    }
}
