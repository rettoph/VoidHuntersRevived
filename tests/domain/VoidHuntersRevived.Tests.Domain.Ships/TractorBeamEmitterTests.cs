using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Events;
using VoidHuntersRevived.Domain.Teams.Common.Components;
using VoidHuntersRevived.Tests.Common.Entities.Extensions;
using VoidHuntersRevived.Tests.Common.Providers;
using VoidHuntersRevived.Tests.Common.Ships.Mockers;
using VoidHuntersRevived.Tests.Common.Simulations;
using VoidHuntersRevived.Tests.Domain.Ships.Mockers;
using VoidHuntersRevived.Tests.Domain.Ships.Stubs;

namespace VoidHuntersRevived.Tests.Domain.Ships
{
    public class TractorBeamEmitterTests
    {
        public readonly ShipSimulationMocker<TestShipLockstepStrategyMocker, TestShipPredictiveStrategyMocker> SimulationMocker;
        public readonly VhIdProvider VhIdProvider;

        public TractorBeamEmitterTests()
        {
            this.SimulationMocker = new ShipSimulationMocker<TestShipLockstepStrategyMocker, TestShipPredictiveStrategyMocker>();
            this.VhIdProvider = new VhIdProvider(VhId.NewVhId());
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

            EntityLocalId predictedShipLocalId = this.SimulationMocker.PredictiveStrategyMocker.EntityQueryServiceBuilder.Object.GetLocalId(shipGlobalId);
            EntityLocalId predictedBridgeLocalId = this.SimulationMocker.PredictiveStrategyMocker.TreeServiceBuilder.Object.GetHead(predictedShipLocalId).LocalId;
            EntityGlobalId predictedBridgeGlobalId = this.SimulationMocker.PredictiveStrategyMocker.EntityQueryServiceBuilder.Object.GetGlobalId(predictedBridgeLocalId);

            // Begin Tests

            // Query for the available piece
            Assert.True(this.SimulationMocker.PredictiveStrategyMocker.TractorBeamEmitterServiceBuilder.Object.Query(
                tractorBeamEmitterLocalId: predictedShipLocalId,
                target: FixVector2.Zero,
                targetNode: out Node targetNode));

            EntityGlobalId targetNodeGlobalId = this.SimulationMocker.PredictiveStrategyMocker.EntityQueryServiceBuilder.Object.GetGlobalId(targetNode.LocalId);

            // "Select" the piece, detaching it from the ship
            this.SimulationMocker.Input(this.VhIdProvider.Next(), new Input_TractorBeamEmitter_Select()
            {
                TractorBeamEmitterGlobalId = shipGlobalId,
                TargetNodeGlobalId = targetNodeGlobalId
            }).Update(TimeSpan.FromMilliseconds(16), 10);

            // Verify state
            this.SimulationMocker.AssertTotalBodies(2).AssertTotalEntities<Tree>(1).AssertTotalEntities<Node>(2);
        }
    }
}
