using Guppy.Attributes;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.EntityComponent.DependencyInjection.Builders;
using Guppy.EntityComponent.Utilities;
using Guppy.Interfaces;
using Guppy.Network.Enums;
using Guppy.ServiceLoaders;
using Minnow.General.Interfaces;
using System;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Library.Globals;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Server.Scenes;

namespace VoidHuntersRevived.Server.ServiceLoaders
{
    [AutoLoad]
    internal sealed class PrimaryServiceLoader : IServiceLoader
    {
        public void RegisterServices(AssemblyHelper assemblyHelper, ServiceProviderBuilder services)
        {
            PeerData.IsServer = true;

            services.RegisterTypeFactory<PrimaryGame>().SetDefaultConstructor<ServerPrimaryGame>();
            services.RegisterTypeFactory<PrimaryScene>().SetDefaultConstructor<ServerPrimaryScene>();

            services.RegisterSetup<Settings>()
                .SetOrder(1)
                .SetMethod((s, p, c) =>
                { // Configure the server settings...
                    s.Set<NetworkAuthorization>(NetworkAuthorization.Master);
                });
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
