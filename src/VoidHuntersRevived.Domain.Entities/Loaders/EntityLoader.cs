using Autofac;
using Guppy.Attributes;
using Guppy.Common.Providers;
using Guppy.Loaders;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Domain.Entities.Engines;
using VoidHuntersRevived.Domain.Entities.Services;

namespace VoidHuntersRevived.Domain.Entities.Loaders
{
    [AutoLoad]
    public sealed class EntityLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder builder)
        {
            builder.RegisterType<EntityTypeService>().As<IEntityTypeService>().AsSelf().InstancePerLifetimeScope();

            builder.RegisterType<SimpleEntitiesSubmissionScheduler>().AsSelf().As<EntitiesSubmissionScheduler>().InstancePerLifetimeScope();

            builder.RegisterType<EnginesRoot>().InstancePerLifetimeScope();

            builder.RegisterType<EngineService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<EntityService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<EntityDescriptorService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<EntitySubmissionEngine>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<EventPublishingService>().AsImplementedInterfaces().InstancePerLifetimeScope();
        }
    }
}
