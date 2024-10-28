using Guppy.Core.Resources.Common;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Common;
using VoidHuntersRevived.Domain.Common.Constants;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Constants;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Events;
using VoidHuntersRevived.Domain.Ships.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Predictive;
using VoidHuntersRevived.Domain.Teams.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Services;
using VoidHuntersRevived.Tests.Common.Physics.Extensions;
using VoidHuntersRevived.Tests.Common.Providers;
using VoidHuntersRevived.Tests.Common.Simulations;
using VoidHuntersRevived.Tests.Common.Simulations.Extensions;
using VoidHuntersRevived.Tests.Domain.Ships;
using VoidHuntersRevived.Tests.Registration.Physics.Extensions;
using VoidHuntersRevived.Tests.Registration.Pieces.Extensions;
using VoidHuntersRevived.Tests.Registration.Ships.Extensions;
using VoidHuntersRevived.Tests.Registration.Simulations.Extensions;
using VoidHuntersRevived.Tests.Registration.Teams.Extensions;

namespace VoidHuntersRevived.Tests.Domain.Pieces
{
    public class TractorBeamEmmiter_AttachDetach_Tests
    {
        public virtual SettingValue<int> StepsPerTick => new(Settings.StepsPerTick, 3);
        public virtual SettingValue<Fix64> StepInterval => new(Settings.StepInterval, (Fix64)20 / (Fix64)1000);

        private static readonly Key<IEntityTemplate> TestSquareEntityTemplateKey = Key<IEntityTemplate>.GetByName(nameof(TestSquareEntityTemplateKey));

        private readonly SimulationMocker _simulation;
        private readonly IStrategyMocker<LockstepStrategy_Client> _lockstep;
        private readonly IStrategyMocker<PredictiveStrategy> _predictive;

        private readonly ITractorBeamEmitterService _readTractorbeamEmitterService;
        private readonly IEntityQueryService _readEntityQueryService;

        public TractorBeamEmmiter_AttachDetach_Tests() : base()
        {
            var builder = new SimulationBuilder(
                id: VhId.Empty,
                stepInterval: StepInterval,
                stepsPerTick: StepsPerTick,
                entityTemplateFragments: this.GetEntityTemplateFragments()
            );

            builder.AddPredictiveStrategy()
                 .AddLockstepClientStrategy()
                 .AddTeamsConfiguration()
                 .AddPhysicsConfiguration()
                 .AddPiecesConfiguration()
                 .AddShipsConfiguration();

            _simulation = builder.Build();
            _predictive = _simulation.Get<PredictiveStrategy>();
            _lockstep = _simulation.Get<LockstepStrategy_Client>();

            _readTractorbeamEmitterService = _predictive.Provider.Get<ITractorBeamEmitterService>();
            _readEntityQueryService = _predictive.Provider.Get<IEntityQueryService>();
        }

        [Fact]
        public void Test1()
        {
            VhId shipVhId = VhId.NewId();

            IEnumerator<int> SetupStrategy(VhIdProvider vhids, IStrategyMocker strategy)
            {
                ITeamService teamService = strategy.Provider.Get<ITeamService>();
                ITreeService treeService = strategy.Provider.Get<ITreeService>();
                ISocketService socketService = strategy.Provider.Get<ISocketService>();

                // Spawn a test ship
                Team team = teamService.GetOpenTeam();
                EntityId shipId = treeService.Spawn(
                    sourceId: vhids.Next(),
                    vhid: shipVhId,
                    team: team,
                    treeTemplateKey: Resources.EntityTemplates.Ship.UserShipEntityTemplate,
                    headNodeTemplateKey: TestSquareEntityTemplateKey);

                yield return 100;

                // Spawn a test square attached to the test ship
                Node head = treeService.GetHead(shipId);
                bool result = socketService.TryGetSocket(new SocketVhId(head.Id.VhId, 0), out NodeSocket nodeSocket);
                Assert.True(result);

                EntityId square = socketService.Spawn(
                    sourceId: vhids.Next(),
                    targetSocketNode: nodeSocket,
                    vhid: vhids.Next(),
                    nodeTemplateKey: TestSquareEntityTemplateKey);

                yield return 100;

                // Verify setup
                strategy.AssertBodyCount(1).AssertEntityCount<Tree>(1).AssertEntityCount<Node>(2);
            }

            // Setup test (create ship with piece attached)
            _simulation.RunCoroutine(
                interval: 16,
                coroutineId: VhId.HashString(nameof(SetupStrategy)),
                coroutine: SetupStrategy);
            EntityId shipId = _readEntityQueryService.GetId(shipVhId);
            EntityId bridgeId = _predictive.Provider.Get<ITreeService>().GetHead(shipId).Id;

            // Begin Tests
            VhIdProvider sourceIdProvider = new(VhId.HashString(nameof(Test1)));

            for (int i = 0; i < 10; i++)
            {
                // "Select" piece, detaching it from the ship
                bool result = _readTractorbeamEmitterService.Query(shipId, FixVector2.Zero, out Node targetNode);
                Assert.True(result);
                _simulation.Input(sourceIdProvider.Next(), new Input_TractorBeamEmitter_Select()
                {
                    ShipVhId = shipVhId,
                    TargetVhId = targetNode.Id.VhId
                }, true).Update(1, 2);

                // "Deselect" the piece, attaching it back onto the ship
                _simulation.Input(sourceIdProvider.Next(), new Input_TractorBeamEmitter_Deselect()
                {
                    ShipVhId = shipVhId,
                    AttachToSocketVhId = new SocketVhId(bridgeId.VhId, 0)
                }, true).Update(1, 2);
            }

            _simulation.Update(16, 1000);

            // Verify state
            _simulation.AssertBodyCount(2).AssertEntityCount<Tree>(2).AssertEntityCount<Node>(2);
        }

        private EntityTemplateFragment[] GetEntityTemplateFragments()
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