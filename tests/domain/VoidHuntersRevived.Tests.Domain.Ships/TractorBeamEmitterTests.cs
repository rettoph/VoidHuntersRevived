using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Events;
using VoidHuntersRevived.Domain.Simulations.Common.Strategies;
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
                // Spawn a test ship
                Team openTeam = strategy.TeamServiceBuilder.Object.GetOpenTeam();
                data.ShipLocalId = strategy.TreeServiceBuilder.Object.Spawn(
                    sourceId: vhids.Next(),
                    globalId: shipGlobalId,
                    team: openTeam,
                    treeTemplateKey: Resources.EntityTemplates.Ship.UserShipEntityTemplate,
                    headNodeTemplateKey: TestResources.TestSquareEntityTemplateKey);
            }, out MockData<TestShipData> data);

            this.SimulationMocker.Update(TimeSpan.FromMilliseconds(16), 100);

            this.SimulationMocker.Invoke(data, (data, vhids, strategy) =>
            {
                // Spawn a test square attached to the test ship
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

            // Load some predictive values
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
            }, true).Update(TimeSpan.FromMilliseconds(16), 10);

            // Verify state
            this.SimulationMocker.AssertTotalBodies(2).AssertTotalEntities<Tree>(2).AssertTotalEntities<Node>(2);

            // "Deselect" the piece, allowing it to float in free space
            this.SimulationMocker.Input(this.VhIdProvider.Next(), new Input_TractorBeamEmitter_Deselect()
            {
                TractorBeamEmitterGlobalId = shipGlobalId,
                AttachToNodeSocketGlobalId = null
            }, true).Update(TimeSpan.FromMilliseconds(16), 10);

            // Verify state
            this.SimulationMocker.AssertTotalBodies(2).AssertTotalEntities<Tree>(2).AssertTotalEntities<Node>(2);

            // Query for the available piece
            Assert.True(this.SimulationMocker.PredictiveStrategyMocker.TractorBeamEmitterServiceBuilder.Object.Query(
                tractorBeamEmitterLocalId: predictedShipLocalId,
                target: FixVector2.Zero,
                targetNode: out targetNode));

            targetNodeGlobalId = this.SimulationMocker.PredictiveStrategyMocker.EntityQueryServiceBuilder.Object.GetGlobalId(targetNode.LocalId);

            // "Select" piece, removing it from free space
            this.SimulationMocker.Input(this.VhIdProvider.Next(), new Input_TractorBeamEmitter_Select()
            {
                TractorBeamEmitterGlobalId = shipGlobalId,
                TargetNodeGlobalId = targetNodeGlobalId
            }, true).Update(TimeSpan.FromMilliseconds(16), 10);

            // Verify state
            this.SimulationMocker.AssertTotalBodies(2).AssertTotalEntities<Tree>(2).AssertTotalEntities<Node>(2);

            // "Deselect" the piece, attaching it back onto the ship
            this.SimulationMocker.Input(this.VhIdProvider.Next(), new Input_TractorBeamEmitter_Deselect()
            {
                TractorBeamEmitterGlobalId = shipGlobalId,
                AttachToNodeSocketGlobalId = new NodeSocketGlobalId(predictedBridgeGlobalId, 0)
            }, true).Update(TimeSpan.FromMilliseconds(16), 10);

            // Verify state
            this.SimulationMocker.AssertTotalBodies(1).AssertTotalEntities<Tree>(1).AssertTotalEntities<Node>(2);
        }

        [Fact]
        public void SpamSelectDeselectWithAttach_Tests()
        {
            EntityGlobalId shipGlobalId = VhId.NewVhId().ToGlobalEntityId();

            this.SimulationMocker.Invoke((data, vhids, strategy) =>
            {
                // Spawn a test ship
                Team openTeam = strategy.TeamServiceBuilder.Object.GetOpenTeam();
                data.ShipLocalId = strategy.TreeServiceBuilder.Object.Spawn(
                    sourceId: vhids.Next(),
                    globalId: shipGlobalId,
                    team: openTeam,
                    treeTemplateKey: Resources.EntityTemplates.Ship.UserShipEntityTemplate,
                    headNodeTemplateKey: TestResources.TestSquareEntityTemplateKey);
            }, out MockData<TestShipData> data);

            this.SimulationMocker.Update(TimeSpan.FromMilliseconds(16), 100);

            this.SimulationMocker.Invoke(data, (data, vhids, strategy) =>
            {
                // Spawn a test square attached to the test ship
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

            // Load some predictive values
            EntityLocalId predictedShipLocalId = this.SimulationMocker.PredictiveStrategyMocker.EntityQueryServiceBuilder.Object.GetLocalId(shipGlobalId);
            EntityLocalId predictedBridgeLocalId = this.SimulationMocker.PredictiveStrategyMocker.TreeServiceBuilder.Object.GetHead(predictedShipLocalId).LocalId;
            EntityGlobalId predictedBridgeGlobalId = this.SimulationMocker.PredictiveStrategyMocker.EntityQueryServiceBuilder.Object.GetGlobalId(predictedBridgeLocalId);

            // Begin Tests

            for (int i = 0; i < 100; i++)
            {
                bool verified = i % 2 == 0;

                // Query for the available piece
                Assert.True(this.SimulationMocker.PredictiveStrategyMocker.TractorBeamEmitterServiceBuilder.Object.Query(
                    tractorBeamEmitterLocalId: predictedShipLocalId,
                    target: FixVector2.Zero,
                    targetNode: out Node targetNode));

                EntityGlobalId targetNodeGlobalId = this.SimulationMocker.PredictiveStrategyMocker.EntityQueryServiceBuilder.Object.GetGlobalId(targetNode.LocalId);

                // "Select" piece, detaching it from the ship
                this.SimulationMocker.Input(this.VhIdProvider.Next(), new Input_TractorBeamEmitter_Select()
                {
                    TractorBeamEmitterGlobalId = shipGlobalId,
                    TargetNodeGlobalId = targetNodeGlobalId
                }, verified).Update(TimeSpan.FromMilliseconds(1), 2);

                // "Deselect" the piece, attaching it back onto the ship
                this.SimulationMocker.Input(this.VhIdProvider.Next(), new Input_TractorBeamEmitter_Deselect()
                {
                    TractorBeamEmitterGlobalId = shipGlobalId,
                    AttachToNodeSocketGlobalId = new NodeSocketGlobalId(predictedBridgeGlobalId, 0)
                }, verified).Update(TimeSpan.FromMilliseconds(1), 2);
            }

            // Ensure the piece is dropped
            this.SimulationMocker.Update(TimeSpan.FromMilliseconds(16), 1000)
                .Input(
                    sourceId: this.VhIdProvider.Next(),
                    input: new Input_TractorBeamEmitter_Deselect()
                    {
                        TractorBeamEmitterGlobalId = shipGlobalId,
                        AttachToNodeSocketGlobalId = new NodeSocketGlobalId(predictedBridgeGlobalId, 0)
                    },
                    verified: true)
                .Update(TimeSpan.FromMilliseconds(16), 1000);

            // Verify state
            this.SimulationMocker.AssertTotalBodies(1).AssertTotalEntities<Tree>(1).AssertTotalEntities<Node>(2);
        }

        [Fact]
        public void SynchPredictiveWithAttachmentFromLockstep_Test()
        {
            // Imagine the following ship
            // 1. Predictive: [Bridge][Square:A][Square:B]
            // 2. Predictive: [Bridge][Square:A] <-DETACH-> [Square:Z]
            // 3. Predictive: [Bridge][Square:A][Square:Y]

            // After such a case stratigies are laid out like so:
            // Predictive: [Bridge][Square:A][Square:Y]
            // Lockstep:   [Bridge][Square:A][Square:B]

            // Now we will detach from both:
            // Predictive: [Bridge] <-DETACH-> [Square:C][Square:X]
            // Lockstep:   [Bridge] <-DETACH-> [Square:C][Square:D]

            // Now connect both
            // Predictive: [Bridge][Square:E][Square:W]
            // Lockstep:   [Bridge][Square:E][Square:F]

            // Wait for predictive syncronization. What will happen?
            // [Square:W] was created out of phase and should be reverted
            // This works, but we had a bug where the deserization method had
            // a reference to a lockstep scoped entity (service references in an entity serializer)
            // this caused incorrect deserialize data - a predictive node would attempt to attach to a
            // lockstep tree

            EntityGlobalId shipGlobalId = VhId.NewVhId().ToGlobalEntityId();
            EntityGlobalId square1GlobalId = new(VhId.NewVhId());
            EntityGlobalId square2GlobalId = new(VhId.NewVhId());

            this.SimulationMocker.Invoke((data, vhids, strategy) =>
            {
                // Spawn a test ship
                Team openTeam = strategy.TeamServiceBuilder.Object.GetOpenTeam();
                data.ShipLocalId = strategy.TreeServiceBuilder.Object.Spawn(
                    sourceId: vhids.Next(),
                    globalId: shipGlobalId,
                    team: openTeam,
                    treeTemplateKey: Resources.EntityTemplates.Ship.UserShipEntityTemplate,
                    headNodeTemplateKey: TestResources.TestSquareEntityTemplateKey);
            }, out MockData<TestShipData> data);

            this.SimulationMocker.Update(TimeSpan.FromMilliseconds(16), 100);

            this.SimulationMocker.Invoke(data, (data, vhids, strategy) =>
            {
                // Spawn a test square attached to the test ship
                Node head = strategy.TreeServiceBuilder.Object.GetHead(data.ShipLocalId);
                Assert.True(strategy.NodeSocketServiceBuilder.Object.TryGetNodeSocket(
                    nodeSocketLocalId: new NodeSocketLocalId(head.LocalId, 0),
                    nodeSocket: out NodeSocket nodeSocket));

                EntityLocalId square1LocalId = strategy.NodeSocketServiceBuilder.Object.Spawn(
                    sourceId: vhids.Next(),
                    targetNodeSocket: nodeSocket,
                    globalId: square1GlobalId,
                    nodeTemplateKey: TestResources.TestSquareEntityTemplateKey);
            });

            this.SimulationMocker.Update(TimeSpan.FromMilliseconds(16), 100);

            this.SimulationMocker.Invoke(data, (data, vhids, strategy) =>
            {
                // Spawn a second test square attached to the first test square
                Assert.True(strategy.NodeSocketServiceBuilder.Object.TryGetNodeSocket(
                    nodeSocketGlobalId: new NodeSocketGlobalId(square1GlobalId, 0),
                    nodeSocket: out NodeSocket nodeSocket));

                EntityLocalId square2LocalId = strategy.NodeSocketServiceBuilder.Object.Spawn(
                    sourceId: vhids.Next(),
                    targetNodeSocket: nodeSocket,
                    globalId: square2GlobalId,
                    nodeTemplateKey: TestResources.TestSquareEntityTemplateKey);
            });

            this.SimulationMocker.Update(TimeSpan.FromMilliseconds(16), 100);

            this.SimulationMocker.AssertTotalBodies(1).AssertTotalEntities<Tree>(1).AssertTotalEntities<Node>(3);

            // Load some predictive values
            EntityLocalId predictedShipLocalId = this.SimulationMocker.PredictiveStrategyMocker.EntityQueryServiceBuilder.Object.GetLocalId(shipGlobalId);

            // Begin Tests

            // Query for the available piece
            Assert.True(this.SimulationMocker.PredictiveStrategyMocker.TractorBeamEmitterServiceBuilder.Object.Query(
                tractorBeamEmitterLocalId: predictedShipLocalId,
                target: FixVector2.Zero,
                targetNode: out Node targetNode));


            // Detach and reattach square2 on the predictive strategy
            this.SimulationMocker
                .Input<IPredictiveStrategy>(this.VhIdProvider.Next(), new Input_TractorBeamEmitter_Select()
                {
                    TractorBeamEmitterGlobalId = shipGlobalId,
                    TargetNodeGlobalId = square2GlobalId
                }, true).Update(TimeSpan.FromMilliseconds(1), 2)
                .Input<IPredictiveStrategy>(this.VhIdProvider.Next(), new Input_TractorBeamEmitter_Deselect()
                {
                    TractorBeamEmitterGlobalId = shipGlobalId,
                    AttachToNodeSocketGlobalId = new NodeSocketGlobalId(square1GlobalId, 0)
                }, true).Update(TimeSpan.FromMilliseconds(1), 2);


            // Select square1 on both strategies
            this.SimulationMocker.Input(this.VhIdProvider.Next(), new Input_TractorBeamEmitter_Select()
            {
                TractorBeamEmitterGlobalId = shipGlobalId,
                TargetNodeGlobalId = square1GlobalId
            }, true).Update(TimeSpan.FromMilliseconds(16), 2);

            // Update all simulations - hopefully the fake drop will resync on the predictive strategy
            this.SimulationMocker.Update(TimeSpan.FromMilliseconds(16), 1000);

            // Verify state
            this.SimulationMocker.AssertTotalBodies(2).AssertTotalEntities<Tree>(2).AssertTotalEntities<Node>(3);
        }
    }
}
