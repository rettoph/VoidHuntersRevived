using Guppy.Core.Logging.Common;
using Guppy.Tests.Common;
using VoidHuntersRevived.Domain.Entities.Services;

namespace VoidHuntersRevived.Tests.Common.Entities.Builders
{
    public class EntitySerializationServiceBuilder : Builder<EntitySerializationService>
    {
        public required EntityTemplateServiceBuilder EntityTemplateServiceBuilder { get; init; }
        public required EntityQueryServiceBuilder EntityQueryServiceBuilder { get; init; }
        public required EntitySpawnServiceBuilder EntitySpawnServiceBuilder { get; init; }
        public required Mocker<ILogger> LoggerMocker { get; init; }

        protected override EntitySerializationService Build()
        {
            return new EntitySerializationService(
                entityTemplateService: this.EntityTemplateServiceBuilder.Object,
                entityQueryService: this.EntityQueryServiceBuilder.Object,
                entitySpawnService: this.EntitySpawnServiceBuilder.Object,
                logger: this.LoggerMocker.Object);
        }
    }
}
