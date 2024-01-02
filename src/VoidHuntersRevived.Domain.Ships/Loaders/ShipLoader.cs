using Autofac;
using Guppy.Attributes;
using Guppy.Loaders;
using VoidHuntersRevived.Domain.Ships.Services;

namespace VoidHuntersRevived.Domain.Ships.Loaders
{
    [AutoLoad]
    public sealed class ShipLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder services)
        {
            services.RegisterType<TractorBeamEmitterService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            services.RegisterType<TacticalService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            services.RegisterType<UserShipService>().AsImplementedInterfaces().InstancePerLifetimeScope();
        }
    }
}
