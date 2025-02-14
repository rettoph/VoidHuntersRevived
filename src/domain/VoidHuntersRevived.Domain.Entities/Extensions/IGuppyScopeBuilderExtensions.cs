using Autofac;
using Guppy.Core.Common;
using Guppy.Core.Common.Extensions;
using Guppy.Core.Common.Providers;
using Guppy.Core.Common.Services;
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
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Entities.Extensions
{
    public static class IGuppyScopeBuilderExtensions
    {
        public static IGuppyScopeBuilder RegisterDomainEntityServices(this IGuppyScopeBuilder builder)
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

                    builder.RegisterType<ComponentSerializerService>().As<IComponentSerializerService>().AsSelf().InstancePerLifetimeScope();

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


                    // This should only really happen when unit testing
                    // Otherwise there should always be a parent scope (boot, global, ect)
                    if (builder.ParentScope is not null)
                    {
                        // Auto register an engine to dispose of instances as needed
                        foreach (Type disposableComponent in builder.ParentScope.Resolve<IAssemblyService>().GetTypes<IEntityComponent>())
                        {
                            if (disposableComponent.IsAssignableTo<IDisposable>())
                            {
                                builder.RegisterType(typeof(DisposableSystem<>).MakeGenericType(disposableComponent))
                                    .As<IEngine>()
                                    .InstancePerLifetimeScope();
                            }
                        }
                    }
                });
            });
        }
    }
}