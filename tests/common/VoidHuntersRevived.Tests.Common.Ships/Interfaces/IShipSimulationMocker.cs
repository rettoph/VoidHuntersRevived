using VoidHuntersRevived.Tests.Common.Physics.Interfaces;
using VoidHuntersRevived.Tests.Common.Ships.Mockers;

namespace VoidHuntersRevived.Tests.Common.Ships.Interfaces
{
    public interface IShipSimulationMocker : IPhysicsSimulationMocker
    {
        ShipLockstepStrategyMocker ShipLockstepStrategyMocker { get; }
        ShipPredictiveStrategyMocker ShipPredictiveStrategyMocker { get; }
    }
    public interface IShipSimulationMocker<out TSelf, TStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker> : IPhysicsSimulationMocker<TSelf, TStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker>, IShipSimulationMocker
        where TSelf : IShipSimulationMocker<TSelf, TStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker>
        where TStrategyMocker : IShipStrategyMocker
        where TLockstepStrategyMocker : ShipLockstepStrategyMocker, TStrategyMocker, new()
        where TPredictiveStrategyMocker : ShipPredictiveStrategyMocker, TStrategyMocker, new()
    {
    }
}
