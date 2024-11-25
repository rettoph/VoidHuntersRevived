using Autofac;
using Guppy.Core.Common.Attributes;
using VoidHuntersRevived.Game.Server.Components.Scene;
using VoidHuntersRevived.Game.Server.Guppy;

namespace VoidHuntersRevived.Game.Server.Modules
{
    [AutoLoad]
    public class ServerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<ConfigureSimulationsComponent>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<ServerPeerComponent>().AsImplementedInterfaces().InstancePerLifetimeScope();
        }
    }
}
