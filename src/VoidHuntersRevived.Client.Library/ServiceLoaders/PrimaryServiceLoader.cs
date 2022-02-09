using Guppy.Attributes;
using Guppy.EntityComponent.DependencyInjection.Builders;
using Guppy.ServiceLoaders;
using System;
using VoidHuntersRevived.Client.Library.Scenes;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Client.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class PrimaryServiceLoader : IServiceLoader
    {
        public void RegisterServices(AssemblyHelper assemblyHelper, ServiceProviderBuilder services)
        {
            services.RegisterTypeFactory<PrimaryGame>().SetMethod(p => new ClientPrimaryGame());
            services.RegisterTypeFactory<PrimaryScene>().SetMethod(p => new ClientPrimaryScene());
        }
    }
}
