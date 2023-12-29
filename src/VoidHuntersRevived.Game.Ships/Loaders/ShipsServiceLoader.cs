using Autofac;
using Guppy.Attributes;
using Guppy.Loaders;
using VoidHuntersRevived.Game.Ships.Services;

namespace VoidHuntersRevived.Game.Ships.Loaders
{
    [AutoLoad]
    public sealed class ShipsServiceLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder services)
        {
            services.RegisterType<TractorBeamEmitterService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            services.RegisterType<TacticalService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            services.RegisterType<UserShipService>().AsImplementedInterfaces().InstancePerLifetimeScope();
        }
    }
}
