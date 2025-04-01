using Guppy.Core.Logging.Common;
using Guppy.Tests.Common;
using VoidHuntersRevived.Domain.Ships.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Services;
using VoidHuntersRevived.Tests.Common.Entities.Builders;
using VoidHuntersRevived.Tests.Common.Physics.Builders;
using VoidHuntersRevived.Tests.Common.Teams.Builders;

namespace VoidHuntersRevived.Tests.Common.Ships.Builders
{
    public class TractorBeamEmitterServiceBuilder : Builder<TractorBeamEmitterService>
    {
        public required IBuilder<IStepEventService> StepEventServiceBuilder { get; init; }
        public required SpaceBuilder SpaceBuilder { get; init; }
        public required EntityQueryServiceBuilder EntityQueryServiceBuilder { get; init; }
        public required EntitySpawnServiceBuilder EntitySpawnServiceBuilder { get; init; }
        public required EntitySerializationServiceBuilder EntitySerializationServiceBuilder { get; init; }
        public required NodeServiceBuilder NodeServiceBuilder { get; init; }
        public required TreeServiceBuilder TreeServiceBuilder { get; init; }
        public required NodeSocketServiceBuilder NodeSockerServiceBuilder { get; init; }
        public required TeamServiceBuilder TeamServiceBuilder { get; init; }
        public required Mocker<ILogger> LoggerMocker { get; init; }

        protected override TractorBeamEmitterService Build()
        {
            return new TractorBeamEmitterService(
                stepEventService: this.StepEventServiceBuilder.Object,
                space: this.SpaceBuilder.Object,
                entityQueryService: this.EntityQueryServiceBuilder.Object,
                entitySpawnService: this.EntitySpawnServiceBuilder.Object,
                entitySerializationService: this.EntitySerializationServiceBuilder.Object,
                nodeService: this.NodeServiceBuilder.Object,
                treeService: this.TreeServiceBuilder.Object,
                socketService: this.NodeSockerServiceBuilder.Object,
                teamService: this.TeamServiceBuilder.Object,
                logger: this.LoggerMocker.Object);
        }
    }
}
