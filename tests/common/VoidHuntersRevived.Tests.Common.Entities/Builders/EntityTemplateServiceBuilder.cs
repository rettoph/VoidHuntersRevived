using Guppy.Core.Common.Services;
using Guppy.Core.Logging.Common.Services;
using Guppy.Tests.Common;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Extensions.Svelto;
using VoidHuntersRevived.Domain.Entities.Services;
using VoidHuntersRevived.Tests.Common.Builders;
using VoidHuntersRevived.Tests.Common.Entities.Mockers;

namespace VoidHuntersRevived.Tests.Common.Entities.Builders
{
    public class EntityTemplateServiceBuilder : Builder<EntityTemplateService>
    {
        public required UniqueNumberProviderBuilder UniqueNumberProviderBuilder { get; init; }
        public required EntityTemplateFragmentServiceMocker EntityTemplateFragmentServiceMocker { get; init; }
        public required Mocker<ILoggerService> LoggerServiceMocker { get; init; }
        public required ComponentSerializerServiceBuilder ComponentSerializerServiceBuilder { get; init; }
        public required Mocker<IScopedSystemService> ScopedSystemServiceMocker { get; init; }
        public required EnginesRoot EnginesRoot { get; init; }

        protected override EntityTemplateService Build()
        {
            return new EntityTemplateService(
                uniqueNumberProvider: this.UniqueNumberProviderBuilder.Object,
                entityTemplateFragmentService: this.EntityTemplateFragmentServiceMocker.Object,
                loggerService: this.LoggerServiceMocker.Object,
                componentSerializerService: this.ComponentSerializerServiceBuilder.Object,
                scopedSystemService: this.ScopedSystemServiceMocker.Object,
                enginesRoot: this.EnginesRoot,
                entitiesDb: this.EnginesRoot.GetEntitiesDB());
        }
    }
}
