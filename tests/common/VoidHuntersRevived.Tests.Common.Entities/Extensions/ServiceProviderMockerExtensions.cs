using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Services;
using VoidHuntersRevived.Tests.Common.Entities.Services;

namespace VoidHuntersRevived.Tests.Common.Entities.Extensions
{
    public static class ServiceProviderMockerExtensions
    {
        public static void AddEntityServices(this ServiceProviderMocker services, IEnumerable<EntityTemplateFragment> entityTemplateFragments)
        {
            EntityService entityService = new(
                services.GetLazy<IEntityTemplateService>(),
                services.GetLazy<IEntityQueryService>(),
                services.GetLazy<IEntitySpawnService>(),
                services.GetLazy<IEntitySerializationService>());

            EntityTemplateFragmentServiceMocker templateFragmentService = new();
            templateFragmentService.AddFragments(entityTemplateFragments);

            EntityTemplateService templateService = new(
                services.Get<IUniqueNumberProvider>(),
                templateFragmentService.GetInstance(),
                services.GetLazy<IComponentSerializerService>(),
                services.Get<EnginesRoot>());

            EntityQueryService queryService = new();

            EntitySpawnService spawnService = new(
                queryService,
                templateService,
                entityService,
                services.Get<ILogger>());


            EntityWriter writer = new(
                templateService,
                queryService,
                services.Get<ILogger>());

            EntityReader reader = new(
                templateService,
                queryService,
                spawnService,
                services.Get<ILogger>());

            EntitySerializationService serializationService = new(writer, reader);

            services.AddRange([
                entityService,
                templateFragmentService,
                templateService,
                queryService,
                spawnService,
                writer,
                reader,
                serializationService
            ]);
        }
    }
}
