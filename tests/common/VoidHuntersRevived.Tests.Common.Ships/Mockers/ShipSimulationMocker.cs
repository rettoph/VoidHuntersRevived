using VoidHuntersRevived.Tests.Common.Physics.Mockers;
using VoidHuntersRevived.Tests.Common.Ships.Interfaces;

namespace VoidHuntersRevived.Tests.Common.Ships.Mockers
{
    public abstract class ShipSimulationMocker<TSelf, TStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker> : PhysicsSimulationMocker<TSelf, TStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker>, IShipSimulationMocker<TSelf, TStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker>
        where TSelf : ShipSimulationMocker<TSelf, TStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker>
        where TStrategyMocker : IShipStrategyMocker
        where TLockstepStrategyMocker : ShipLockstepStrategyMocker, TStrategyMocker, new()
        where TPredictiveStrategyMocker : ShipPredictiveStrategyMocker, TStrategyMocker, new()
    {
        ShipLockstepStrategyMocker IShipSimulationMocker.ShipLockstepStrategyMocker => this.LockstepStrategyMocker;
        ShipPredictiveStrategyMocker IShipSimulationMocker.ShipPredictiveStrategyMocker => this.PredictiveStrategyMocker;
    }

    public sealed class ShipSimulationMocker<TLockstepStrategyMocker, TPredictiveStrategyMocker> : ShipSimulationMocker<ShipSimulationMocker<TLockstepStrategyMocker, TPredictiveStrategyMocker>, IShipStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker>
        where TLockstepStrategyMocker : ShipLockstepStrategyMocker, IShipStrategyMocker, new()
        where TPredictiveStrategyMocker : ShipPredictiveStrategyMocker, IShipStrategyMocker, new()
    {
    }
}
