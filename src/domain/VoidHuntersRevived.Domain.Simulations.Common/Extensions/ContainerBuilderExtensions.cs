using Autofac;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Simulations.Common.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder RegisterEngine<T>(this ContainerBuilder builder)
            where T : IEngine
        {
            builder.RegisterType<T>().AsImplementedInterfaces().InstancePerLifetimeScope();

            return builder;
        }
    }
}
