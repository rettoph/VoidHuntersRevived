using VoidHuntersRevived.Tests.Common.Physics.Interfaces;
using VoidHuntersRevived.Tests.Common.Teams.Mockers;

namespace VoidHuntersRevived.Tests.Common.Physics.Mockers
{
    public abstract class PhysicsSimulationMocker<TSelf, TStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker> : TeamSimulationMocker<TSelf, TStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker>, IPhysicsSimulationMocker<TSelf, TStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker>
        where TSelf : PhysicsSimulationMocker<TSelf, TStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker>
        where TStrategyMocker : IPhysicsStrategyMocker
        where TLockstepStrategyMocker : PhysicsLockstepStrategyMocker, TStrategyMocker, new()
        where TPredictiveStrategyMocker : PhysicsPredictiveStrategyMocker, TStrategyMocker, new()
    {
        PhysicsLockstepStrategyMocker IPhysicsSimulationMocker.PhysicsLockstepStrategyMocker => this.LockstepStrategyMocker;
        PhysicsPredictiveStrategyMocker IPhysicsSimulationMocker.PhysicsPredictiveStrategyMocker => this.PredictiveStrategyMocker;
    }

    public sealed class PhysicsSimulationMocker<TLockstepStrategyMocker, TPredictiveStrategyMocker> : PhysicsSimulationMocker<PhysicsSimulationMocker<TLockstepStrategyMocker, TPredictiveStrategyMocker>, IPhysicsStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker>
        where TLockstepStrategyMocker : PhysicsLockstepStrategyMocker, IPhysicsStrategyMocker, new()
        where TPredictiveStrategyMocker : PhysicsPredictiveStrategyMocker, IPhysicsStrategyMocker, new()
    {
    }
}
