using Guppy.Attributes;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.EntityComponent.DependencyInjection.Builders;
using Guppy.ServiceLoaders;
using System;
using VoidHuntersRevived.Client.Library.Components.Players;
using VoidHuntersRevived.Library.Entities.Players;

namespace VoidHuntersRevived.Client.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class PlayerServiceLoader : IServiceLoader
    {
        public void RegisterServices(AssemblyHelper assemblyHelper, ServiceProviderBuilder services)
        {
            services.RegisterComponent<UserPlayerCurrentUserCameraComponent>()
                .SetAssignableEntityType<UserPlayer>()
                .RegisterService(service =>
                {
                    service.RegisterTypeFactory(factory =>
                    {
                        factory.SetDefaultConstructor<UserPlayerCurrentUserCameraComponent>();
                    });
                });
        }
    }
}
