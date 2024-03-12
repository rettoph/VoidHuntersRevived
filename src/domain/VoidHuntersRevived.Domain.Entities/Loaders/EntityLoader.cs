using Autofac;
using Guppy.Attributes;
using Guppy.Extensions.Autofac;
using Guppy.Loaders;
using Serilog;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Common.Entities;
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
            builder.RegisterType<ComponentSerializerService>().As<IComponentSerializerService>().AsSelf().InstancePerLifetimeScope();

            builder.RegisterType<EntityTypeService>().As<IEntityTypeService>().AsSelf().InstancePerLifetimeScope();

            builder.RegisterType<EntityTypeInitializerService>().As<IEntityTypeInitializerService>().AsSelf().InstancePerLifetimeScope();

            builder.RegisterType<SimpleEntitiesSubmissionScheduler>().AsSelf().As<EntitiesSubmissionScheduler>().InstancePerLifetimeScope();

            builder.RegisterType<EnginesRoot>().InstancePerLifetimeScope();

            builder.RegisterType<EngineService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<EntityService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<EntitySubmissionEngine>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<VoidHuntersEntityDescriptorConverter>().As<JsonConverter>().SingleInstance();

            builder.Configure<LoggerConfiguration>((scope, config) =>
            {
                config.Destructure.AsScalar(typeof(Id<IEntityComponent>));
                config.Destructure.AsScalar(typeof(Id<IEntityType>));
            });
        }
    }
}
