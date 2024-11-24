using Autofac;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;

namespace VoidHuntersRevived.Domain.Entities.Common.Exceptions
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder RegisterComponentSerializer<T>(this ContainerBuilder builder)
            where T : ComponentSerializer
        {
            builder.RegisterType<T>().As<ComponentSerializer>().InstancePerLifetimeScope();


            return builder;
        }
    }
}
