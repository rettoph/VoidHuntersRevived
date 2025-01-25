using Autofac;
using Guppy.Core.Common;
using Guppy.Core.Common.Extensions;
using VoidHuntersRevived.Game.Server.Components.Scene;
using VoidHuntersRevived.Game.Server.Guppy;

namespace VoidHuntersRevived.Game.Server.Extensions
{
    public static class IGuppyScopeBuilderExtensions
    {
        public static IGuppyScopeBuilder RegisterGameServerServices(this IGuppyScopeBuilder builder)
        {
            return builder.EnsureRegisteredOnce(nameof(RegisterGameServerServices), builder =>
            {
                builder.RegisterType<ConfigureSimulationsComponent>().AsImplementedInterfaces().InstancePerLifetimeScope();
                builder.RegisterType<ServerPeerComponent>().AsImplementedInterfaces().InstancePerLifetimeScope();
            });
        }
    }
}