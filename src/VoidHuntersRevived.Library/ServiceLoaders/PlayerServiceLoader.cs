using Guppy.Attributes;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.EntityComponent.DependencyInjection.Builders;
using Guppy.Interfaces;
using Guppy.Network.Builders;
using Guppy.ServiceLoaders;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Components.Players;
using VoidHuntersRevived.Library.Components.Players.UserPlayers;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Globals.Constants;
using VoidHuntersRevived.Library.Messages.Network;
using VoidHuntersRevived.Library.Messages.Network.Packets;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class PlayerServiceLoader : IServiceLoader, INetworkLoader
    {
        public void ConfigureNetwork(NetworkProviderBuilder network)
        {
            network.RegisterNetworkEntityMessage<UserPlayerDirectionRequestMessage>()
                .SetDeliveryMethod(DeliveryMethod.ReliableOrdered);

            network.RegisterNetworkEntityMessage<UserPlayerTargetRequestMessage>()
                .SetDeliveryMethod(DeliveryMethod.Unreliable);

            network.RegisterNetworkEntityMessage<UserPlayerTractorBeamStateRequestMessage>()
                .SetDeliveryMethod(DeliveryMethod.Unreliable);

            network.RegisterDataType<UserPlayerCreatePacket>()
                .SetReader(UserPlayerCreatePacket.Read)
                .SetWriter(UserPlayerCreatePacket.Write);
        }

        public void RegisterServices(AssemblyHelper assemblyHelper, ServiceProviderBuilder services)
        {
            services.RegisterService<PlayerService>()
                .SetLifetime(ServiceLifetime.Scoped)
                .RegisterTypeFactory(factory => factory.SetDefaultConstructor<PlayerService>());

            services.RegisterEntity<UserPlayer>()
                .RegisterService(service =>
                {
                    service.SetLifetime(ServiceLifetime.Transient)
                        .RegisterTypeFactory(factory => factory.SetDefaultConstructor<UserPlayer>());
                })
                .RegisterComponent<UserPlayerMasterCRUDComponent>(component =>
                {
                    component.RegisterService(service =>
                    {
                        service.RegisterTypeFactory(factory => factory.SetDefaultConstructor<UserPlayerMasterCRUDComponent>());
                    });
                })
                .RegisterComponent<UserPlayerSlaveCRUDComponent>(component =>
                {
                    component.RegisterService(service =>
                    {
                        service.RegisterTypeFactory(factory => factory.SetDefaultConstructor<UserPlayerSlaveCRUDComponent>());
                    });
                })
                .RegisterComponent<CurrentUserPlayerTractorBeamComponent>(component =>
                {
                    component.RegisterService(service =>
                    {
                        service.RegisterTypeFactory(factory => factory.SetDefaultConstructor<CurrentUserPlayerTractorBeamComponent>());
                    });
                })
                .RegisterComponent<CurrentUserPlayerTractorBeamRemoteSlaveComponent>(component =>
                {
                    component.RegisterService(service =>
                    {
                        service.RegisterTypeFactory(factory => factory.SetDefaultConstructor<CurrentUserPlayerTractorBeamRemoteSlaveComponent>());
                    });
                })
                .RegisterComponent<CurrentUserPlayerTractorBeamRemoteMasterComponent>(component =>
                {
                    component.RegisterService(service =>
                    {
                        service.RegisterTypeFactory(factory => factory.SetDefaultConstructor<CurrentUserPlayerTractorBeamRemoteMasterComponent>());
                    });
                })
                .RegisterComponent<CurrentUserPlayerTargetComponent>(component =>
                {
                    component.RegisterService(service =>
                    {
                        service.RegisterTypeFactory(factory => factory.SetDefaultConstructor<CurrentUserPlayerTargetComponent>());
                    });
                })
                .RegisterComponent<CurrentUserPlayerTargetRemoteSlaveComponent>(component =>
                {
                    component.RegisterService(service =>
                    {
                        service.RegisterTypeFactory(factory => factory.SetDefaultConstructor<CurrentUserPlayerTargetRemoteSlaveComponent>());
                    });
                })
                .RegisterComponent<CurrentUserPlayerTargetRemoteMasterComponent>(component =>
                {
                    component.RegisterService(service =>
                    {
                        service.RegisterTypeFactory(factory => factory.SetDefaultConstructor<CurrentUserPlayerTargetRemoteMasterComponent>());
                    });
                })
                .RegisterComponent<CurrentUserPlayerDirectionComponent>(component =>
                {
                    component.RegisterService(service =>
                    {
                        service.RegisterTypeFactory(factory => factory.SetDefaultConstructor<CurrentUserPlayerDirectionComponent>());
                    });
                })
                .RegisterComponent<CurrentUserPlayerDirectionRemoteSlaveComponent>(component =>
                {
                    component.RegisterService(service =>
                    {
                        service.RegisterTypeFactory(factory => factory.SetDefaultConstructor<CurrentUserPlayerDirectionRemoteSlaveComponent>());
                    });
                })
                .RegisterComponent<CurrentUserPlayerDirectionRemoteMasterComponent>(component =>
                {
                    component.RegisterService(service =>
                    {
                        service.RegisterTypeFactory(factory => factory.SetDefaultConstructor<CurrentUserPlayerDirectionRemoteMasterComponent>());
                    });
                });
                // .RegisterComponent<UserPlayerCurrentUserThrustComponent>(component =>
                // {
                //     component.RegisterService(service =>
                //     {
                //         service.RegisterTypeFactory(factory => factory.SetDefaultConstructor<UserPlayerCurrentUserThrustComponent>());
                //     });
                // })
                // .RegisterComponent<UserPlayerCurrentUserTractorBeamComponent>(component =>
                // {
                //     component.RegisterService(service =>
                //     {
                //         service.RegisterTypeFactory(factory => factory.SetDefaultConstructor<UserPlayerCurrentUserTractorBeamComponent>());
                //     });
                // })
                // .RegisterComponent<UserPlayerCurrentUserTargetingComponent>(component =>
                // {
                //     component.RegisterService(service =>
                //     {
                //         service.RegisterTypeFactory(factory => factory.SetDefaultConstructor<UserPlayerCurrentUserTargetingComponent>());
                //     });
                // });

            services.RegisterComponent<PlayerPipeComponent>()
                .SetAssignableEntityType<Player>()
                .RegisterService(service =>
                {
                    service.RegisterTypeFactory(factory =>
                    {
                        factory.SetDefaultConstructor<PlayerPipeComponent>();
                    });
                });
        }
    }
}
