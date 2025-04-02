using Guppy.Core.Resources.Common.Services;
using Guppy.Tests.Common;
using VoidHuntersRevived.Domain.Pieces.Serialization.Components;
using VoidHuntersRevived.Domain.Pieces.Systems;
using VoidHuntersRevived.Domain.Ships.Serialization.Components;
using VoidHuntersRevived.Domain.Ships.Systems;
using VoidHuntersRevived.Tests.Common.Physics.Mockers;
using VoidHuntersRevived.Tests.Common.Ships.Builders;
using VoidHuntersRevived.Tests.Common.Ships.Interfaces;

namespace VoidHuntersRevived.Tests.Common.Ships.Mockers
{
    public class ShipPredictiveStrategyMocker : PhysicsPredictiveStrategyMocker, IShipStrategyMocker
    {
        public BlueprintServiceBuilder BlueprintServiceBuilder { get; }
        public TreeServiceBuilder TreeServiceBuilder { get; }
        public NodeServiceBuilder NodeServiceBuilder { get; }
        public NodeSocketServiceBuilder NodeSocketServiceBuilder { get; }
        public TractorBeamEmitterServiceBuilder TractorBeamEmitterServiceBuilder { get; }
        public TacticalServiceBuilder TacticalServiceBuilder { get; }
        public Mocker<IResourceService> ResourceServiceMocker { get; }

        public ShipPredictiveStrategyMocker()
        {
            this.ResourceServiceMocker = new Mocker<IResourceService>();
            this.BlueprintServiceBuilder = new BlueprintServiceBuilder()
            {
                Blueprints = [],
                ResourceServiceMocker = this.ResourceServiceMocker
            };
            this.TreeServiceBuilder = new TreeServiceBuilder()
            {
                EntityQueryServiceBuilder = this.EntityQueryServiceBuilder,
                EntitySpawnServiceBuilder = this.EntitySpawnServiceBuilder
            };
            this.NodeServiceBuilder = new NodeServiceBuilder()
            {
                EntityQueryServiceBuilder = this.EntityQueryServiceBuilder
            };
            this.NodeSocketServiceBuilder = new NodeSocketServiceBuilder()
            {
                EntityQueryServiceBuilder = this.EntityQueryServiceBuilder,
                EntitySpawnServiceBuilder = this.EntitySpawnServiceBuilder,
                EntitySerializationServiceBuilder = this.EntitySerializationServiceBuilder,
                LoggerMocker = this.LoggerMocker,
            };
            this.TractorBeamEmitterServiceBuilder = new TractorBeamEmitterServiceBuilder()
            {
                StepEventServiceBuilder = this.PredictiveStepEventServiceBuilder,
                SpaceBuilder = this.SpaceBuilder,
                EntityQueryServiceBuilder = this.EntityQueryServiceBuilder,
                EntitySpawnServiceBuilder = this.EntitySpawnServiceBuilder,
                EntitySerializationServiceBuilder = this.EntitySerializationServiceBuilder,
                NodeServiceBuilder = this.NodeServiceBuilder,
                TreeServiceBuilder = this.TreeServiceBuilder,
                NodeSockerServiceBuilder = this.NodeSocketServiceBuilder,
                TeamServiceBuilder = this.TeamServiceBuilder,
                LoggerMocker = this.LoggerMocker
            };
            this.TacticalServiceBuilder = new TacticalServiceBuilder()
            {
                EntityQueryServiceBuilder = this.EntityQueryServiceBuilder
            };

            this.ComponentSerializerServiceBuilder.ComponentSerializers.AddRange([
                () => new CouplingComponentSerializer(
                    entityQueryService: this.EntityQueryServiceBuilder.Object),
                () => new NodeComponentSerializer(
                    entityQueryService: this.EntityQueryServiceBuilder.Object,
                    logger: this.LoggerMocker.Object),
                () => new PlugComponentSerializer(),
                () => new SocketIdsComponentSerializer(
                    entityQueryService: this.EntityQueryServiceBuilder.Object,
                    socketService: this.NodeSocketServiceBuilder.Object),
                () => new TreeComponentSerializer(),
                () => new HelmComponentSerializer(),
                () => new TacticalComponentSerializer(),
                () => new TractorableComponentSerializer(),
                () => new UserIdComponentSerializer()
            ]);

            this.SystemFactories.AddRange([
                x => new CouplingSystem(
                    socketService: this.NodeSocketServiceBuilder.Object),
                x => new NodeFixtureSystem(
                    strategy: x,
                    entityQueryService: this.EntityQueryServiceBuilder.Object,
                    entitySpawnService: this.EntitySpawnServiceBuilder.Object,
                    socketService: this.NodeSocketServiceBuilder.Object,
                    logger: this.LoggerMocker.Object),
                x => new SocketIdsSystem(
                    entityQueryService: this.EntityQueryServiceBuilder.Object,
                    entitySpawnService: this.EntitySpawnServiceBuilder.Object,
                    socketService: this.NodeSocketServiceBuilder.Object),
                x => new ThrustableSystem(
                    entityQueryService: this.EntityQueryServiceBuilder.Object,
                    space: this.SpaceBuilder.Object),
                x => new TractorableSystem(
                    tacticalService: this.TacticalServiceBuilder.Object,
                    entityQueryService: this.EntityQueryServiceBuilder.Object,
                    logger: this.LoggerMocker.Object),
                x => new TreeSystem(
                    entitySpawnService: this.EntitySpawnServiceBuilder.Object,
                    logger: this.LoggerMocker.Object),
                x => new UserIdSystem(
                    entityQueryService: this.EntityQueryServiceBuilder.Object),
                x => new HelmSystem(
                    entityQueryService: this.EntityQueryServiceBuilder.Object),
                x => new TacticalSystem(
                    entityQueryService: this.EntityQueryServiceBuilder.Object),
                x => new TractorBeamEmitterServiceEventSystem(
                    nodeSocketService: this.NodeSocketServiceBuilder.Object,
                    treeService: this.TreeServiceBuilder.Object,
                    teamService: this.TeamServiceBuilder.Object,
                    logger: this.LoggerMocker.Object),
                x => new TractorBeamEmitterInputSystem(
                    tractorBeamEmitterService: this.TractorBeamEmitterServiceBuilder.Object),
                x => new TractorBeamEmitterUpdateSystem(
                    entityQueryService: this.EntityQueryServiceBuilder.Object,
                    space: this.SpaceBuilder.Object,
                    logger: this.LoggerMocker.Object,
                    socketService: this.NodeSocketServiceBuilder.Object)
            ]);
        }
    }
}
