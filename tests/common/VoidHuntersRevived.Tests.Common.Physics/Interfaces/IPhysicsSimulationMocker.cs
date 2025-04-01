using VoidHuntersRevived.Tests.Common.Entities.Interfaces;
using VoidHuntersRevived.Tests.Common.Physics.Mockers;

namespace VoidHuntersRevived.Tests.Common.Physics.Interfaces
{
    public interface IPhysicsSimulationMocker : IEntitySimulationMocker
    {
        PhysicsLockstepStrategyMocker PhysicsLockstepStrategyMocker { get; }
        PhysicsPredictiveStrategyMocker PhysicsPredictiveStrategyMocker { get; }
    }
    public interface IPhysicsSimulationMocker<out TSelf, TStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker> : IEntitySimulationMocker<TSelf, TStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker>, IPhysicsSimulationMocker
        where TSelf : IPhysicsSimulationMocker<TSelf, TStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker>
        where TStrategyMocker : IPhysicsStrategyMocker
        where TLockstepStrategyMocker : PhysicsLockstepStrategyMocker, TStrategyMocker, new()
        where TPredictiveStrategyMocker : PhysicsPredictiveStrategyMocker, TStrategyMocker, new()
    {
    }
}
