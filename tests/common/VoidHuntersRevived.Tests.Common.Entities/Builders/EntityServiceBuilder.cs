using Guppy.Tests.Common;
using Guppy.Tests.Common.Extensions;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Services;

namespace VoidHuntersRevived.Tests.Common.Entities.Builders
{
    public class EntityServiceBuilder : Builder<EntityService>
    {
        public required EntityTemplateServiceBuilder EntityTemplateServiceBuilder { get; init; }
        public required EntityQueryServiceBuilder EntityQueryServiceBuilder { get; init; }
        public required EntitySpawnServiceBuilder EntitySpawnServiceBuilder { get; init; }
        public required EntitySerializationServiceBuilder EntitySerializationServiceBuilder { get; init; }

        protected override EntityService Build()
        {
            return new EntityService(
                entityTemplateService: this.EntityTemplateServiceBuilder.Object.ToLazy<IEntityTemplateService>(),
                entityQueryService: this.EntityQueryServiceBuilder.Object.ToLazy<IEntityQueryService>(),
                entitySpawnService: this.EntitySpawnServiceBuilder.Object.ToLazy<IEntitySpawnService>(),
                entitySerialzationService: this.EntitySerializationServiceBuilder.Object.ToLazy<IEntitySerializationService>());
        }
    }
}
