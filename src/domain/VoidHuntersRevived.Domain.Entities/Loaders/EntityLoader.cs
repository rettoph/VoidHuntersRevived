using Autofac;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Extensions.Autofac;
using Guppy.Core.Serialization.Common.Converters;
using Guppy.Engine.Common.Loaders;
using Serilog;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Engines;
using VoidHuntersRevived.Domain.Entities.Serialization.Json;
using VoidHuntersRevived.Domain.Entities.Services;

namespace VoidHuntersRevived.Domain.Entities.Loaders
{
    [AutoLoad]
    public sealed class EntityLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder builder)
        {
            builder.RegisterType<EntityReader>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<EntityWriter>().AsSelf().InstancePerLifetimeScope();

            builder.RegisterType<ComponentSerializerService>().As<IComponentSerializerService>().AsSelf().InstancePerLifetimeScope();

            builder.RegisterType<EntityTemplateFragmentService>().AsImplementedInterfaces().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<EntityTemplateService>().AsImplementedInterfaces().AsSelf().InstancePerLifetimeScope();

            builder.RegisterType<EntitiesSubmissionScheduler>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<EnginesRoot>().InstancePerLifetimeScope();

            builder.RegisterType<EntityService>().AsSelf().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<EntityQueryService>().AsSelf().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<EntitySpawnService>().AsSelf().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<EntitySerializationService>().AsSelf().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<EntitySubmissionEngine>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<EntityTemplateConverter>().As<JsonConverter>().SingleInstance();
            builder.RegisterType<ResourceComponentConverter>().As<JsonConverter>().SingleInstance();
            builder.RegisterType<DictionaryPolymorphicConverter<IEntityComponent>>().As<JsonConverter>().SingleInstance();

            builder.Configure<LoggerConfiguration>((scope, config) =>
            {
                config.Destructure.AsScalar(typeof(Id<IEntityComponent>));
                config.Destructure.AsScalar(typeof(Id<EntityTemplateFragment>));
            });
        }
    }
}
