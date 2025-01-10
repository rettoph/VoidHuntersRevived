using Autofac;
using Guppy.Core.Common.Extensions.Autofac;
using VoidHuntersRevived.Game.Server.Components.Scene;
using VoidHuntersRevived.Game.Server.Guppy;

namespace VoidHuntersRevived.Game.Server.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder RegisterGameServerServices(this ContainerBuilder builder)
        {
            return builder.EnsureRegisteredOnce(nameof(RegisterGameServerServices), builder =>
                                                                                                             {
                                                                                                                 builder.RegisterType<ConfigureSimulationsComponent>().AsImplementedInterfaces().InstancePerLifetimeScope();
                                                                                                                 builder.RegisterType<ServerPeerComponent>().AsImplementedInterfaces().InstancePerLifetimeScope();
                                                                                                             });
        }
    }
}