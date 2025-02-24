using Autofac;
using Guppy.Core.Common.Builders;
using Guppy.Core.Common.Extensions;
using Guppy.Core.Common.Providers;
using Guppy.Core.Resources.Common.Extensions;
using Guppy.Core.Serialization.Common.Converters;
using Guppy.Core.Serialization.Common.Extensions;
using Guppy.Game.Common.Extensions;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Extensions.Svelto;
using VoidHuntersRevived.Domain.Entities.Providers;
using VoidHuntersRevived.Domain.Entities.ResourceTypes;
using VoidHuntersRevived.Domain.Entities.Serialization.Json;
using VoidHuntersRevived.Domain.Entities.Services;
using VoidHuntersRevived.Domain.Entities.Systems;
using VoidHuntersRevived.Domain.Simulations.Common.Strategies;

namespace VoidHuntersRevived.Domain.Entities.Extensions
{
    public static class IGuppyRootBuilderExtensions
    {
        public static IGuppyRootBuilder RegisterDomainEntityServices(this IGuppyRootBuilder builder)
        {
            return builder.EnsureRegisteredOnce(nameof(RegisterDomainEntityServices), builder =>
            {
                builder.RegisterJsonConverter<EntityTemplateConverter>();
                builder.RegisterJsonConverter<ResourceComponentConverter>();
                builder.RegisterJsonConverter<DictionaryPolymorphicConverter<IEntityComponent>>();

                builder.RegisterResourceType<EntityTemplateFragmentResourceType>();

                // Automoq will attempt to resolve a mocked instance unless a default registration
                // is defined. Since there is no public constrctor on this type the end result
                // is an unexpected exception. Just adding a registration manually here - even if its one we
                // never intent to call - fixes that issue.
                builder.Register<EntitiesDB>(ctx => throw new NotImplementedException());

                builder.RegisterType<EntityTemplateFragmentService>().As<IEntityTemplateFragmentService>().SingleInstance();

                builder.RegisterSceneFilter<IStrategy>(builder =>
                {
                    builder.RegisterSceneSystem<EntitySubmissionSystem>();
                    builder.RegisterSceneSystem<InitializeEntityServicesSystem>();

                    builder.RegisterType<ComponentSerializerService>().AsSelf().As<IComponentSerializerService>().AsSelf().InstancePerLifetimeScope();
                    builder.RegisterType<EntityTemplateService>().AsSelf().As<IEntityTemplateService>().InstancePerLifetimeScope();

                    builder.RegisterType<EntitiesSubmissionScheduler>().AsSelf().InstancePerLifetimeScope();
                    builder.RegisterType<EnginesRoot>().InstancePerLifetimeScope();
                    builder.Register<EntitiesDB>(ctx => ctx.Resolve<EnginesRoot>().GetEntitiesDB()).InstancePerLifetimeScope();

                    builder.RegisterType<EntityService>().As<IEntityService>().InstancePerLifetimeScope();
                    builder.RegisterType<EntityQueryService>().AsSelf().As<IEntityQueryService>().InstancePerLifetimeScope();
                    builder.RegisterType<EntitySpawnService>().As<IEntitySpawnService>().As<IPrivateEntitySpawnService>().InstancePerLifetimeScope();
                    builder.RegisterType<EntitySerializationService>().As<IEntitySerializationService>().InstancePerLifetimeScope();

                    builder.RegisterSceneSystem<EngineSystem>();
                    builder.RegisterSceneSystem<EntitySpawnServiceEventSystem>();
                    builder.RegisterType<BelongsToSystemProvider>().As<IScopedSystemProvider>().InstancePerLifetimeScope();
                    builder.RegisterType<DisposableSystemProvider>().As<IScopedSystemProvider>().InstancePerLifetimeScope();
                });
            });
        }
    }
}