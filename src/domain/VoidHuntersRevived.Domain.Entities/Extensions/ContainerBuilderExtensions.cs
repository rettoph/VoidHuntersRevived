using Autofac;
using Guppy.Core.Common.Extensions.Autofac;
using Guppy.Core.Resources.Common.Extensions.Autofac;
using Guppy.Core.Serialization.Common.Converters;
using Guppy.Core.Serialization.Common.Extensions;
using Serilog;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Engines;
using VoidHuntersRevived.Domain.Entities.Providers;
using VoidHuntersRevived.Domain.Entities.ResourceTypes;
using VoidHuntersRevived.Domain.Entities.Serialization.Json;
using VoidHuntersRevived.Domain.Entities.Services;

namespace VoidHuntersRevived.Domain.Entities.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder RegisterDomainEntityServices(this ContainerBuilder builder)
        {
            return builder.EnsureRegisteredOnce(nameof(RegisterDomainEntityServices), builder =>
                                                                                                               {
                                                                                                                   builder.RegisterType<ComponentSerializerService>().As<IComponentSerializerService>().AsSelf().InstancePerLifetimeScope();

                                                                                                                   builder.RegisterType<EntityTemplateFragmentService>().AsImplementedInterfaces().AsSelf().InstancePerLifetimeScope();
                                                                                                                   builder.RegisterType<EntityTemplateService>().AsImplementedInterfaces().AsSelf().InstancePerLifetimeScope();

                                                                                                                   builder.RegisterType<EntitiesSubmissionScheduler>().AsSelf().InstancePerLifetimeScope();
                                                                                                                   builder.RegisterType<EnginesRoot>().InstancePerLifetimeScope();

                                                                                                                   builder.RegisterType<EngineService>().As<IEngineService>().InstancePerLifetimeScope();

                                                                                                                   builder.RegisterType<EntityService>().AsSelf().AsImplementedInterfaces().InstancePerLifetimeScope();
                                                                                                                   builder.RegisterType<EntityQueryService>().AsSelf().AsImplementedInterfaces().InstancePerLifetimeScope();
                                                                                                                   builder.RegisterType<EntitySpawnService>().AsSelf().AsImplementedInterfaces().InstancePerLifetimeScope();
                                                                                                                   builder.RegisterType<EntitySerializationService>().AsSelf().AsImplementedInterfaces().InstancePerLifetimeScope();

                                                                                                                   builder.RegisterType<EntitySubmissionEngine>().AsImplementedInterfaces().InstancePerLifetimeScope();

                                                                                                                   builder.RegisterJsonConverter<EntityTemplateConverter>();
                                                                                                                   builder.RegisterJsonConverter<ResourceComponentConverter>();
                                                                                                                   builder.RegisterJsonConverter<DictionaryPolymorphicConverter<IEntityComponent>>();

                                                                                                                   builder.RegisterResourceType<EntityTemplateFragmentResourceType>();

                                                                                                                   builder.RegisterType<BelongsToEngineProvider>().As<IEngineProvider>().InstancePerLifetimeScope();

                                                                                                                   const string EntityLoggerContext = "Entities";
                                                                                                                   builder.RegisterLoggerContext<EntityQueryService>(EntityLoggerContext);
                                                                                                                   builder.RegisterLoggerContext<EntitySerializationService>(EntityLoggerContext);
                                                                                                                   builder.RegisterLoggerContext<EntitySpawnService>(EntityLoggerContext);
                                                                                                                   builder.RegisterLoggerContext<EntityTemplateFragmentService>(EntityLoggerContext);
                                                                                                                   builder.RegisterLoggerContext<ComponentSerializerService>(EntityLoggerContext);
                                                                                                                   builder.RegisterLoggerContext<EntityTemplate>(EntityLoggerContext);

                                                                                                                   builder.Configure<LoggerConfiguration>((scope, config) =>
                                                                                                                   {
                                                                                                                       config.Destructure.AsScalar(typeof(Id<IEntityComponent>));
                                                                                                                       config.Destructure.AsScalar(typeof(Id<EntityTemplateFragment>));
                                                                                                                       config.Destructure.AsScalar(typeof(EntityLocalId));
                                                                                                                       config.Destructure.AsScalar(typeof(EntityGlobalId));
                                                                                                                   });
                                                                                                               });
        }
    }
}