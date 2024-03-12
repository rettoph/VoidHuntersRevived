using Autofac;
using Guppy.Attributes;
using Guppy.Loaders;
using VoidHuntersRevived.Domain.Client.Services;
using VoidHuntersRevived.Game.Client.Graphics.Effects;

namespace VoidHuntersRevived.Domain.Client.Loaders
{
    [AutoLoad]
    public sealed class ClientLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder services)
        {
            services.RegisterType<VisibleInstanceRenderingService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            services.RegisterType<VisibleEffect>().InstancePerDependency();
        }
    }
}
