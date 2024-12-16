using Autofac;
using Guppy.Core.Resources.Common;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Common;
using VoidHuntersRevived.Domain.Common.Constants;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Physics.Extensions;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Constants;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Extensions;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Events;
using VoidHuntersRevived.Domain.Ships.Common.Services;
using VoidHuntersRevived.Domain.Ships.Extensions;
using VoidHuntersRevived.Domain.Simulations.Common.Predictive;
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Predictive;
using VoidHuntersRevived.Domain.Teams.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Services;
using VoidHuntersRevived.Domain.Teams.Extensions;
using VoidHuntersRevived.Tests.Common.Physics.Extensions;
using VoidHuntersRevived.Tests.Common.Providers;
using VoidHuntersRevived.Tests.Common.Simulations;
using VoidHuntersRevived.Tests.Common.Simulations.Extensions;
using VoidHuntersRevived.Tests.Domain.Ships;

namespace VoidHuntersRevived.Tests.Domain.Pieces
{
    public class TractorBeamEmmiter_AttachDetach_Tests
    {
        public static SettingValue<int> StepsPerTick => new(Settings.StepsPerTick, 3);
        public static SettingValue<Fix64> StepInterval => new(Settings.StepInterval, (Fix64)20 / (Fix64)1000);

        private static readonly Key<IEntityTemplate> TestSquareEntityTemplateKey = Key<IEntityTemplate>.GetByName(nameof(TestSquareEntityTemplateKey));

        private static SimulationMocker CreateSimulationMocker()
        {
            return new SimulationBuilder(
                    id: VhId.Empty,
                    stepInterval: StepInterval,
                    stepsPerTick: StepsPerTick,
                    entityTemplateFragments: GetEntityTemplateFragments()
                )
                .AddStrategy<PredictiveStrategy>()
                .AddStrategy<LockstepStrategy_Client>()
                .Register(builder =>
                {
                    builder.RegisterDomainTeamsServices();
                    builder.RegisterDomainPhysicsServices();
                    builder.RegisterDomainPiecesServices();
                    builder.RegisterDomainShipsServices();

                    builder.RegisterInstance(Enumerable.Empty<Blueprint>());
                })
                .Build();
        }

        [Fact]
        public void SpamSelectDeselectWithAttach_Tests()
        {
            var simulation = CreateSimulationMocker();
            var readTractorbeamEmitterService = simulation.Resolve<PredictiveStrategy, ITractorBeamEmitterService>();
            var readEntityQueryService = simulation.Resolve<PredictiveStrategy, IEntityQueryService>();
            var readTreeService = simulation.Resolve<PredictiveStrategy, ITreeService>();

            EntityGlobalId shipGlobalId = VhId.NewId().ToGlobalEntityId();

            IEnumerator<int> SetupStrategy(VhIdProvider vhids, IStrategyMocker strategy)
            {
                ITeamService teamService = strategy.Scope.Resolve<ITeamService>();
                ITreeService treeService = strategy.Scope.Resolve<ITreeService>();
                INodeSocketService socketService = strategy.Scope.Resolve<INodeSocketService>();

                // Spawn a test ship
                Team team = teamService.GetOpenTeam();
                EntityId shipId = treeService.Spawn(
                    sourceId: vhids.Next(),
                    globalId: shipGlobalId,
                    team: team,
                    treeTemplateKey: Resources.EntityTemplates.Ship.UserShipEntityTemplate,
                    headNodeTemplateKey: TestSquareEntityTemplateKey);

                yield return 100;

                // Spawn a test square attached to the test ship
                Node head = treeService.GetHead(shipId.ToLocalEntityId());
                bool result = socketService.TryGetNodeSocket(new NodeSocketLocalId(head.LocalId, 0), out NodeSocket nodeSocket);
                Assert.True(result);

                EntityId squareId = socketService.Spawn(
                    sourceId: vhids.Next(),
                    targetSocketNode: nodeSocket,
                    globalId: vhids.Next().ToGlobalEntityId(),
                    nodeTemplateKey: TestSquareEntityTemplateKey);

                yield return 100;

                // Verify setup
                strategy.AssertBodyCount(1).AssertEntityCount<Tree>(1).AssertEntityCount<Node>(2);
            }

            // Setup test (create ship with piece attached)
            simulation.RunCoroutine(
                interval: TimeSpan.FromMilliseconds(16),
                coroutineId: VhId.HashString(nameof(SetupStrategy)),
                coroutine: SetupStrategy);
            EntityLocalId shipLocalId = readEntityQueryService.GetLocalId(shipGlobalId);
            EntityLocalId bridgeLocalId = readTreeService.GetHead(shipLocalId).LocalId;
            EntityGlobalId bridgeGlobalId = readEntityQueryService.GetGlobalId(bridgeLocalId);

            // Begin Tests
            VhIdProvider sourceIdProvider = new(VhId.HashString(nameof(SpamSelectDeselectWithAttach_Tests)));

            for (int i = 0; i < 100; i++)
            {
                bool verified = i % 2 == 0;

                // Query for the available piece
                bool result = readTractorbeamEmitterService.Query(shipLocalId, FixVector2.Zero, out Node targetNode);
                Assert.True(result);

                EntityGlobalId targetNodeGlobalId = readEntityQueryService.GetGlobalId(targetNode.LocalId);

                // "Select" piece, detaching it from the ship
                simulation.Input(sourceIdProvider.Next(), new Input_TractorBeamEmitter_Select()
                {
                    TractorBeamEmitterGlobalId = shipGlobalId,
                    TargetNodeGlobalId = targetNodeGlobalId
                }, verified).Update(TimeSpan.FromMilliseconds(1), 2);

                // "Deselect" the piece, attaching it back onto the ship
                simulation.Input(sourceIdProvider.Next(), new Input_TractorBeamEmitter_Deselect()
                {
                    TractorBeamEmitterGlobalId = shipGlobalId,
                    AttachToNodeSocketGlobalId = new NodeSocketGlobalId(bridgeGlobalId, 0)
                }, verified).Update(TimeSpan.FromMilliseconds(1), 2);
            }

            // Ensure the piece is dropped
            simulation
                .Update(TimeSpan.FromMilliseconds(16), 1000)
                .Input(
                    sourceId: sourceIdProvider.Next(),
                    data: new Input_TractorBeamEmitter_Deselect()
                    {
                        TractorBeamEmitterGlobalId = shipGlobalId,
                        AttachToNodeSocketGlobalId = new NodeSocketGlobalId(bridgeGlobalId, 0)
                    },
                    verified: true)
                .Update(TimeSpan.FromMilliseconds(16), 1000);

            // Verify state
            simulation.AssertBodyCount(1).AssertEntityCount<Tree>(1).AssertEntityCount<Node>(2);
        }

        [Fact]
        public void SynchPredictiveWithAttachmentFromLockstep_Tests()
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
            // this caused inorrect deserialize data - a predictive node would attempt to attach to a
            // lockstep tree

            var simulation = CreateSimulationMocker();
            var readTractorbeamEmitterService = simulation.Resolve<PredictiveStrategy, ITractorBeamEmitterService>();
            var readEntityQueryService = simulation.Resolve<PredictiveStrategy, IEntityQueryService>();
            var readTreeService = simulation.Resolve<PredictiveStrategy, ITreeService>();

            EntityGlobalId shipGlobalId = VhId.NewId().ToGlobalEntityId();
            EntityGlobalId square1Id = new(VhId.NewId());
            EntityGlobalId square2Id = new(VhId.NewId());

            IEnumerator<int> SetupStrategy(VhIdProvider vhids, IStrategyMocker strategy)
            {
                ITeamService teamService = strategy.Scope.Resolve<ITeamService>();
                ITreeService treeService = strategy.Scope.Resolve<ITreeService>();
                INodeSocketService socketService = strategy.Scope.Resolve<INodeSocketService>();

                // Spawn a test ship
                Team team = teamService.GetOpenTeam();
                EntityId shipId = treeService.Spawn(
                    sourceId: vhids.Next(),
                    globalId: shipGlobalId,
                    team: team,
                    treeTemplateKey: Resources.EntityTemplates.Ship.UserShipEntityTemplate,
                    headNodeTemplateKey: TestSquareEntityTemplateKey);

                yield return 100;

                // Spawn a test square attached to the test ship
                Node head = treeService.GetHead(shipId.ToLocalEntityId());
                bool result = socketService.TryGetNodeSocket(new NodeSocketLocalId(head.LocalId, 0), out NodeSocket nodeSocket);
                Assert.True(result);

                EntityId square = socketService.Spawn(
                    sourceId: vhids.Next(),
                    targetSocketNode: nodeSocket,
                    globalId: square1Id,
                    nodeTemplateKey: TestSquareEntityTemplateKey);

                yield return 100;

                // Spawn a second test square attached to the first test square
                result = socketService.TryGetNodeSocket(new NodeSocketGlobalId(square.ToGlobalEntityId(), 0), out nodeSocket);
                Assert.True(result);

                EntityId square2 = socketService.Spawn(
                    sourceId: vhids.Next(),
                    targetSocketNode: nodeSocket,
                    globalId: square2Id,
                    nodeTemplateKey: TestSquareEntityTemplateKey);

                yield return 100;

                // Verify setup
                strategy.AssertBodyCount(1).AssertEntityCount<Tree>(1).AssertEntityCount<Node>(3);
            }

            // Setup test (create ship with piece attached)
            simulation.RunCoroutine(
                interval: TimeSpan.FromMilliseconds(16),
                coroutineId: VhId.HashString(nameof(SetupStrategy)),
                coroutine: SetupStrategy);
            EntityLocalId shipLocalId = readEntityQueryService.GetLocalId(shipGlobalId);

            // Begin Tests
            VhIdProvider sourceIdProvider = new(VhId.HashString(nameof(SpamSelectDeselectWithAttach_Tests)));

            // Query for the available piece
            bool result = readTractorbeamEmitterService.Query(shipLocalId, FixVector2.Zero, out Node targetNode);
            Assert.True(result);


            // Detach and reattach square2 on the predictive strategy
            simulation
                .Input<IPredictiveStrategy>(sourceIdProvider.Next(), new Input_TractorBeamEmitter_Select()
                {
                    TractorBeamEmitterGlobalId = shipGlobalId,
                    TargetNodeGlobalId = square2Id
                }, true).Update(TimeSpan.FromMilliseconds(1), 2)
                .Input<IPredictiveStrategy>(sourceIdProvider.Next(), new Input_TractorBeamEmitter_Deselect()
                {
                    TractorBeamEmitterGlobalId = shipGlobalId,
                    AttachToNodeSocketGlobalId = new NodeSocketGlobalId(square1Id, 0)
                }, true).Update(TimeSpan.FromMilliseconds(1), 2);


            // Select square1 on both strategies
            simulation.Input(sourceIdProvider.Next(), new Input_TractorBeamEmitter_Select()
            {
                TractorBeamEmitterGlobalId = shipGlobalId,
                TargetNodeGlobalId = square1Id
            }, true).Update(TimeSpan.FromMilliseconds(16), 2);

            // Update all simulations - hopefully the fake drop will resync on the predictive strategy
            simulation.Update(TimeSpan.FromMilliseconds(16), 1000);

            // Verify state
            simulation.AssertBodyCount(2).AssertEntityCount<Tree>(2).AssertEntityCount<Node>(3);
        }

        private static EntityTemplateFragment[] GetEntityTemplateFragments()
        {
            return [
                new EntityTemplateFragment()
                {
                    Key = Key<IEntityTemplate>.GetByName("Test.DefaultTeam"),
                    Components = [
                        new DefaultTeam(),
                        new Team(ResourceKey<string>.Get("Test.DefaultTeam"))
                    ]
                },
                new EntityTemplateFragment()
                {
                    Key = Key<IEntityTemplate>.GetByName("Test.Team"),
                    Components = [
                        new Team(ResourceKey<string>.Get("Test.Team")),
                        new ColorScheme(TestResources.Colors.TestColor, TestResources.Colors.TestColor)
                    ]
                },
                new EntityTemplateFragment()
                {
                    Key = Resources.EntityTemplates.Ship.ChainEntityTemplate,
                    Components = [
                        new TeamMember(),
                        new Location(),
                        new Enabled(),
                        new Awake(true),
                        new Collision()
                        {
                            Categories = CollisionGroups.FreeFloatingCategories,
                            CollidesWith = CollisionGroups.FreeFloatingCollidesWith
                        },
                        new Tree(),
                        new Tractorable(),
                    ]
                },
                new EntityTemplateFragment()
                {
                    Key = TestSquareEntityTemplateKey,
                    Components = [
                        new TeamMember(),
                        Plug.Default,
                        new Coupling(),
                        new Node(),
                        new ColorScheme(TestResources.Colors.TestColor, TestResources.Colors.TestColor),
                        new Rigid(TestResources.BodyTemplates.TestSquareBodyTemplate),
                        new Sockets()
                        {
                            Items = new Socket[] {
                                new(new Location(new FixVector2(1, 0.5), Fix64.Zero))
                            }.ToNativeDynamicArray()
                        }
                    ]
                },
                new EntityTemplateFragment()
                {
                    Key = Resources.EntityTemplates.Ship.UserShipEntityTemplate,
                    Components = [
                        new TeamMember(),
                        new Location(),
                        new Enabled(),
                        new Tree(),
                        new Helm(),
                        new Tactical(),
                        new TractorBeamEmitter()
                        {
                            Active = false
                        },
                        new Awake(sleepingAllowed: false),
                        new Collision()
                        {
                            Categories = CollisionGroups.ShipCategories,
                            CollidesWith = CollisionGroups.ShipCollidesWith
                        },
                        new PhysicsBubble()
                        {
                            Enabled = true,
                            Radius = (Fix64)25
                        }
                    ]
                },
            ];
        }
    }
}