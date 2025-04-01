using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
using VoidHuntersRevived.Tests.Common.Ships.Mockers;
using VoidHuntersRevived.Tests.Domain.Ships.Mockers;

namespace VoidHuntersRevived.Tests.Domain.Ships
{
    public class TractorBeamEmitterTests
    {
        public readonly ShipSimulationMocker<TestShipLockstepStrategyMocker, TestShipPredictiveStrategyMocker> SimulationMocker;

        public TractorBeamEmitterTests()
        {
            this.SimulationMocker = new ShipSimulationMocker<TestShipLockstepStrategyMocker, TestShipPredictiveStrategyMocker>();
        }

        [Fact]
        public void SelectDeselectReselectAttach_Test()
        {
            EntityGlobalId shipGlobalId = VhId.NewVhId().ToGlobalEntityId();
        }
    }
}
