using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Components;
using VoidHuntersRevived.Tests.Common.Entities.Extensions;
using VoidHuntersRevived.Tests.Common.Ships.Mockers;
using VoidHuntersRevived.Tests.Common.Simulations;
using VoidHuntersRevived.Tests.Domain.Ships.Mockers;
using VoidHuntersRevived.Tests.Domain.Ships.Stubs;

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

            this.SimulationMocker.Invoke((data, vhids, strategy) =>
            {
                Team openTeam = strategy.TeamServiceBuilder.Object.GetOpenTeam();
                data.ShipLocalId = strategy.TreeServiceBuilder.Object.Spawn(
                    sourceId: vhids.Next(),
                    globalId: shipGlobalId,
                    team: openTeam,
                    treeTemplateKey: Resources.EntityTemplates.Ship.UserShipEntityTemplate,
                    headNodeTemplateKey: TestResources.TestSquareEntityTemplateKey);
            }, out MockData<SelectDeselectReselectAttachData> data);

            this.SimulationMocker.Update(TimeSpan.FromMilliseconds(16), 100);

            this.SimulationMocker.Invoke(data, (data, vhids, strategy) =>
            {
                Node head = strategy.TreeServiceBuilder.Object.GetHead(data.ShipLocalId);
                bool result = strategy.NodeSocketServiceBuilder.Object.TryGetNodeSocket(new NodeSocketLocalId(head.LocalId, 0), out NodeSocket nodeSocket);
                Assert.True(result);

                EntityLocalId squareLocalId = strategy.NodeSocketServiceBuilder.Object.Spawn(
                    sourceId: vhids.Next(),
                    targetNodeSocket: nodeSocket,
                    globalId: vhids.Next().ToGlobalEntityId(),
                    nodeTemplateKey: TestResources.TestSquareEntityTemplateKey);
            });

            this.SimulationMocker.Update(TimeSpan.FromMilliseconds(16), 100);

            this.SimulationMocker.AssertTotalBodies(1).AssertTotalEntities<Tree>(1).AssertTotalEntities<Node>(2);
        }
    }
}
