using Guppy.Core.Logging.Common;
using Guppy.Tests.Common;
using VoidHuntersRevived.Domain.Pieces.Services;
using VoidHuntersRevived.Tests.Common.Entities.Builders;

namespace VoidHuntersRevived.Tests.Common.Ships.Builders
{
    public class NodeSocketServiceBuilder : Builder<NodeSocketService>
    {
        public required EntityQueryServiceBuilder EntityQueryServiceBuilder { get; init; }
        public required EntitySpawnServiceBuilder EntitySpawnServiceBuilder { get; init; }
        public required EntitySerializationServiceBuilder EntitySerializationServiceBuilder { get; init; }
        public required Mocker<ILogger> LoggerMocker { get; init; }

        protected override NodeSocketService Build()
        {
            return new NodeSocketService(
                entityQueryService: this.EntityQueryServiceBuilder.Object,
                entitySpawnService: this.EntitySpawnServiceBuilder.Object,
                entitySerializationService: this.EntitySerializationServiceBuilder.Object,
                logger: this.LoggerMocker.Object);
        }
    }
}
