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

                builder.RegisterSceneFilter<IStrategy>(builder =>
                {
                    builder.RegisterSceneSystem<EntitySubmissionSystem>();
                    builder.RegisterSceneSystem<InitializeEntityServicesSystem>();

                    builder.RegisterType<ComponentSerializerService>().As<IComponentSerializerService>().AsSelf().InstancePerLifetimeScope();

                    builder.RegisterType<EntityTemplateFragmentService>().As<IEntityTemplateFragmentService>().SingleInstance();
                    builder.RegisterType<EntityTemplateService>().AsSelf().AsImplementedInterfaces().InstancePerLifetimeScope();

                    builder.RegisterType<EntitiesSubmissionScheduler>().AsSelf().InstancePerLifetimeScope();
                    builder.RegisterType<EnginesRoot>().InstancePerLifetimeScope();
                    builder.Register<EntitiesDB>(ctx => ctx.Resolve<EnginesRoot>().GetEntitiesDB());

                    builder.RegisterType<EngineService>().As<IEngineService>().InstancePerLifetimeScope();

                    builder.RegisterType<EntityService>().AsSelf().AsImplementedInterfaces().InstancePerLifetimeScope();
                    builder.RegisterType<EntityQueryService>().AsSelf().AsImplementedInterfaces().InstancePerLifetimeScope();
                    builder.RegisterType<EntitySpawnService>().AsSelf().AsImplementedInterfaces().InstancePerLifetimeScope();
                    builder.RegisterType<EntitySerializationService>().AsSelf().AsImplementedInterfaces().InstancePerLifetimeScope();

                    builder.RegisterType<BelongsToSystemProvider>().As<IScopedSystemProvider>().InstancePerLifetimeScope();


                    // This should only really happen when unit testing
                    // Otherwise there should always be a parent scope (boot, global, ect)
                    if (builder.ParentScope is not null)
                    {
                        // Auto register an engine to dispose of instances as needed
                        foreach (Type disposableComponent in builder.ParentScope.ResolveService<IAssemblyService>().GetTypes<IEntityComponent>())
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