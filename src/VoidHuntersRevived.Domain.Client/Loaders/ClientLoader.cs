using Autofac;
using Guppy.Attributes;
using Guppy.Loaders;
using VoidHuntersRevived.Domain.Client.Services;

namespace VoidHuntersRevived.Domain.Client.Loaders
{
    [AutoLoad]
    public sealed class ClientLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder services)
        {
            services.RegisterType<VisibleRenderingService>().AsImplementedInterfaces().InstancePerLifetimeScope();
        }
    }
}
