using Guppy.Core.Resources.Common;
using Guppy.Tests.Common;
using Serilog;
using Svelto.ECS;
using System.Collections;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Physics.Engines;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Constants;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Engines;
using VoidHuntersRevived.Domain.Pieces.Services;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Events;
using VoidHuntersRevived.Domain.Ships.Common.Services;
using VoidHuntersRevived.Domain.Ships.Engines;
using VoidHuntersRevived.Domain.Ships.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Predictive;
using VoidHuntersRevived.Domain.Teams.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Services;
using VoidHuntersRevived.Domain.Teams.Services;
using VoidHuntersRevived.Tests.Common.Simulations;
using VoidHuntersRevived.Tests.Common.Simulations.Extensions;
using VoidHuntersRevived.Tests.Common.Simulations.Strategies;
using VoidHuntersRevived.Tests.Domain.Ships;

namespace VoidHuntersRevived.Tests.Domain.Pieces
{
    public class TractorBeamEmmiter_AttachDetach_Tests : BaseSimulationTests<TractorBeamEmmiter_AttachDetach_Tests>
    {
        private static readonly Key<IEntityTemplate> TestSquareEntityTemplateKey = Key<IEntityTemplate>.GetByName(nameof(TestSquareEntityTemplateKey));

        private readonly LockstepStrategy_Client _lockstep;
        private readonly PredictiveStrategy _predictive;

        private readonly ITractorBeamEmitterService _readTractorBeamEmitterService;
        private readonly IEntityQueryService _readEntityQueryService;
        private readonly ISocketService _readSocketService;

        public TractorBeamEmmiter_AttachDetach_Tests() : base([
            typeof(ClientLockstepStrategyBuilder),
            typeof(PredictiveStrategyBuilder)
        ])
        {
            _predictive = (PredictiveStrategy?)this.simulation[StrategyTypeEnum.Predictive] ?? throw new NotImplementedException();
            _lockstep = (LockstepStrategy_Client?)this.simulation[StrategyTypeEnum.Lockstep] ?? throw new NotImplementedException();

            _readTractorBeamEmitterService = _predictive.Engines.Get<ITractorBeamEmitterService>();
            _readEntityQueryService = _predictive.Engines.Get<IEntityQueryService>();
            _readSocketService = _predictive.Engines.Get<ISocketService>();
        }

        [Theory]
        [InlineData(16)]
        public void Test1(int simulatedRealtimeIntervalInMilliseconds)
        {
            VhId sourceId;

            VhId[] chainVhIds = [
                HashBuilder<VhId, int>.Instance.Calculate(0),
                HashBuilder<VhId, int>.Instance.Calculate(1),
                HashBuilder<VhId, int>.Instance.Calculate(2),
            ];

            IEnumerator<int> SetupStrategy(IStrategy strategy)
            {
                ITeamService teamService = strategy.Engines.Get<ITeamService>();
                ITreeService treeService = strategy.Engines.Get<ITreeService>();
                ISocketService socketService = strategy.Engines.Get<ISocketService>();

                Team team = teamService.GetOpenTeam();

                EntityId shipId = treeService.Spawn(chainVhIds[0], chainVhIds[0], team, Resources.EntityTemplates.Ship.UserShipEntityTemplate, TestSquareEntityTemplateKey);

                yield return 0;
                // this.Update(strategy, simulatedRealtimeIntervalInMilliseconds, 10);
                // yield return 1;
                // 
                // EntityId bridgeId = treeService.GetHead(shipId).Id;
                // if (socketService.TryGetSocket(new SocketVhId(bridgeId.VhId, 0), out NodeSocket nodeSocket) == false)
                // {
                //     throw new NotImplementedException();
                // }
                // EntityId squareId = socketService.Spawn(chainVhIds[1], nodeSocket, chainVhIds[1], TestSquareEntityTemplateKey);
                // 
                // yield return 2;
                // this.Update(strategy, simulatedRealtimeIntervalInMilliseconds, 10);
                // yield return 3;
                // 
                // if (socketService.TryGetSocket(new SocketVhId(squareId.VhId, 0), out nodeSocket) == false)
                // {
                //     throw new NotImplementedException();
                // }
                // squareId = socketService.Spawn(chainVhIds[2], nodeSocket, chainVhIds[2], TestSquareEntityTemplateKey);
                // 
                // yield return 2;
                // this.Update(strategy, simulatedRealtimeIntervalInMilliseconds, 10);
                // yield return 3;
            }

            this.simulation.Run(SetupStrategy);
            this.Update(simulatedRealtimeIntervalInMilliseconds, 100);



            // Test has been setup:
            // 1 'ship'
            // 2 free floating parts

            // Chain '0' is the ship. Cache the id now
            EntityId shipId = _readEntityQueryService.GetId(chainVhIds[0]);
            if (_readTractorBeamEmitterService.Query(shipId, FixVector2.Zero, out Node targetNode) == false)
            {
                throw new NotImplementedException();
            }

            this.simulation.Input(
                sourceId: this.GenerateSourceId(),
                data: new Input_TractorBeamEmitter_Select()
                {
                    ShipVhId = shipId.VhId,
                    TargetVhId = targetNode.Id.VhId
                });
        }

        protected override IEnumerable<IEngine> GetEngines(IStrategyBuilder builder)
        {
            IEntityQueryService entityQueryService = builder.EngineServiceBuilder.Engines.OfType<IEntityQueryService>().Single();
            IEntitySerializationService entitySerializationService = builder.EngineServiceBuilder.Engines.OfType<IEntitySerializationService>().Single();
            IEntitySpawnService entitySpawnService = builder.EngineServiceBuilder.Engines.OfType<IEntitySpawnService>().Single();
            IPrivateEntitySpawnService privateEntitySpawnService = builder.EngineServiceBuilder.Engines.OfType<IPrivateEntitySpawnService>().Single();
            IEntityTemplateService entityTemplateService = builder.EngineServiceBuilder.Engines.OfType<IEntityTemplateService>().Single();
            ILogger logger = builder.Logger.GetInstance();

            List<IEngine> engines = [];

            Space space = new(logger, new World());

            Mocker<IBlueprintService> blueprintService = new();
            TeamService teamService = new(entityTemplateService, entityQueryService, privateEntitySpawnService);
            NodeService nodeService = new(entityQueryService);
            TreeService treeService = new(entityQueryService, entitySpawnService, blueprintService.GetInstance());
            SocketService socketService = new(entityQueryService, entitySpawnService, entitySerializationService, treeService, logger);
            TacticalService tacticalService = new(entityQueryService);
            TractorBeamEmitterService tractorBeamEmitterService = new(space, entityQueryService, entitySpawnService, entitySerializationService, nodeService, treeService, socketService, teamService, logger);

            CouplingEngine couplingEngine = new(socketService, logger);
            NodeEngine nodeEngine = new(entityQueryService, entitySpawnService, socketService, logger);
            RigidEngine rigidEngine = new(space, entityQueryService, logger);
            ThrustableEngine thrustableEngine = new(entityQueryService, space);
            SocketIdsEngine socketIdsEngine = new(entityQueryService, entitySpawnService, socketService, logger);
            TractorableEngine tractorableEngine = new(tractorBeamEmitterService, tacticalService, entityQueryService, logger);
            TreeEngine treeEngine = new(entityQueryService, entitySpawnService, logger);
            TractorBeamEmitterInputEngine tractorBeamEmitterInputEngine = new(tractorBeamEmitterService, entityQueryService, logger);
            TractorBeamEmitterUpdateEngine tractorBeamEmitterUpdateEngine = new(entityQueryService, space, logger, tractorBeamEmitterService, socketService);
            TacticalEngine tacticalEngine = new(entityQueryService);
            BodyAwakeEngine bodyAwakeEngine = new(entityQueryService, logger, space);
            BodyLocationEngine bodyLocationEngine = new(entityQueryService, space);
            BodyPhysicsBubbleEngine bodyPhysicsBubbleEngine = new(entityQueryService, space);
            SpaceEngine spaceEngine = new(space);

            return [
                teamService,
                // nodeService, - Not an engine
                treeService,
                socketService,
                // tacticalService, - Not an engine
                tractorBeamEmitterService,
                couplingEngine,
                nodeEngine,
                rigidEngine,
                thrustableEngine,
                socketIdsEngine,
                tractorableEngine,
                treeEngine,
                tractorBeamEmitterInputEngine,
                tractorBeamEmitterUpdateEngine,
                tacticalEngine,
                bodyAwakeEngine,
                bodyLocationEngine,
                bodyPhysicsBubbleEngine,
                spaceEngine
            ];
        }

        protected override IEnumerable<EntityTemplateFragment> GetEntityTemplateFragments()
        {
            yield return new EntityTemplateFragment()
            {
                Key = Key<IEntityTemplate>.GetByName("Test.DefaultTeam"),
                Components = [
                    new DefaultTeam(),
                    new Team(ResourceKey<string>.Get("Test.DefaultTeam"))
                ]
            };

            yield return new EntityTemplateFragment()
            {
                Key = Key<IEntityTemplate>.GetByName("Test.Team"),
                Components = [
                    new Team(ResourceKey<string>.Get("Test.Team")),
                    new ColorScheme(TestResources.Colors.TestColor, TestResources.Colors.TestColor)
                ]
            };

            yield return new EntityTemplateFragment()
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
            };

            yield return new EntityTemplateFragment()
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
            };

            yield return new EntityTemplateFragment()
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
            };
        }
    }
}