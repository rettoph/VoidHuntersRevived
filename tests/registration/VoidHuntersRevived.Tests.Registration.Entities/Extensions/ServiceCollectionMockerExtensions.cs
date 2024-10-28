using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Services;
using VoidHuntersRevived.Tests.Common;
using VoidHuntersRevived.Tests.Common.Entities.Services;
using VoidHuntersRevived.Tests.Common.Extensions;

namespace VoidHuntersRevived.Tests.Registration.Entities.Extensions
{
    public static class ServiceCollectionMockerExtensions
    {
        public static void RegisterEntityServices(this ServiceCollectionMocker services, IEnumerable<EntityTemplateFragment> entityTemplateFragments)
        {
            EntityTemplateFragmentServiceMocker templateFragmentService = new();
            templateFragmentService.AddFragments(entityTemplateFragments);
            services.RegisterMocker(templateFragmentService);

            services.RegisterFactory<EntityService>(provider => new(
                provider.GetLazy<IEntityTemplateService>(),
                provider.GetLazy<IEntityQueryService>(),
                provider.GetLazy<IEntitySpawnService>(),
                provider.GetLazy<IEntitySerializationService>()));

            services.RegisterFactory<EntityTemplateService>(provider => new(
                provider.Get<IUniqueNumberProvider>(),
                provider.Get<IEntityTemplateFragmentService>(),
                provider.GetLazy<IComponentSerializerService>(),
                provider.Get<EnginesRoot>()));

            services.RegisterFactory<EntityQueryService>(provider => new());

            services.RegisterFactory<EntitySpawnService>(provider => new(
                provider.Get<EntityQueryService>(),
                provider.Get<IEntityTemplateService>(),
                provider.Get<IEntityService>(),
                provider.Get<ILogger>()));

            services.RegisterFactory<EntityWriter>(provider => new(
                provider.Get<IEntityTemplateService>(),
                provider.Get<IEntityQueryService>(),
                provider.Get<ILogger>()));

            services.RegisterFactory<EntityReader>(provider => new(
                provider.Get<IEntityTemplateService>(),
                provider.Get<IEntityQueryService>(),
                provider.Get<IEntitySpawnService>(),
                provider.Get<ILogger>()));

            services.RegisterFactory<EntitySerializationService>(provider => new(
                provider.Get<EntityWriter>(),
                provider.Get<EntityReader>()));

            services.RegisterFactory<ComponentSerializerService>(provider => new(
                provider.GetAll<ComponentSerializer>().ToFiltered()));
        }
    }
}
