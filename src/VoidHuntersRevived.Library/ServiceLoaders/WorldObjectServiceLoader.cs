using Guppy.Attributes;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.EntityComponent.Lists;
using System;
using VoidHuntersRevived.Library.Components.WorldObjects;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Interfaces;
using Guppy.EntityComponent.DependencyInjection.Builders;
using Guppy.ServiceLoaders;
using Guppy.Network.Builders;
using VoidHuntersRevived.Library.Messages.Network.Packets;
using VoidHuntersRevived.Library.Messages.Network;
using LiteNetLib;

namespace VoidHuntersRevived.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class WorldObjectServiceLoader : IServiceLoader, INetworkLoader
    {
        public void ConfigureNetwork(NetworkProviderBuilder network)
        {
            network.RegisterNetworkEntityMessage<WorldObjectPositionPing>()
                .SetDeliveryMethod(DeliveryMethod.Sequenced)
                .SetSequenceChannel(Globals.Constants.SequenceChannels.WorldObjectPositionPingSequenceChannel);

            network.RegisterDataType<WorldObjectPositionPacket>()
                .SetReader(WorldObjectPositionPacket.Read)
                .SetWriter(WorldObjectPositionPacket.Write);

            network.RegisterDataType<AetherBodyWorldObjectVelocityPacket>()
                .SetReader(AetherBodyWorldObjectVelocityPacket.Read)
                .SetWriter(AetherBodyWorldObjectVelocityPacket.Write);
        }

        public void RegisterServices(AssemblyHelper assemblyHelper, ServiceProviderBuilder services)
        {
            services.RegisterService<FrameableList<IWorldObject>>()
                .SetLifetime(ServiceLifetime.Transient)
                .RegisterTypeFactory(factory => factory.SetDefaultConstructor<FrameableList<IWorldObject>>());

            services.RegisterEntity<Chain>()
                .RegisterService(service =>
                {
                    service.SetLifetime(ServiceLifetime.Transient)
                        .RegisterTypeFactory(factory => factory.SetDefaultConstructor<Chain>());
                })
                .RegisterComponent<ChainMasterCRUDComponent>(component =>
                {
                    component.RegisterService(service =>
                    {
                        service.RegisterTypeFactory(factory => factory.SetDefaultConstructor<ChainMasterCRUDComponent>());
                    });
                })
                .RegisterComponent<ChainSlaveCRUDComponent>(component =>
                {
                    component.RegisterService(service =>
                    {
                        service.RegisterTypeFactory(factory => factory.SetDefaultConstructor<ChainSlaveCRUDComponent>());
                    });
                });

            services.RegisterEntity<IWorldObject>()
                .RegisterComponent<WorldObjectChunkComponent>(component =>
                {
                    component.RegisterService(service =>
                    {
                        service.RegisterTypeFactory(factory => factory.SetDefaultConstructor<WorldObjectChunkComponent>());
                    });
                })
                .RegisterComponent<WorldObjectMasterCRUDComponent>(component =>
                {
                    component.RegisterService(service =>
                    {
                        service.RegisterTypeFactory(factory => factory.SetDefaultConstructor<WorldObjectMasterCRUDComponent>());
                    });
                })
                .RegisterComponent<WorldObjectSlaveCRUDComponent>(component =>
                {
                    component.RegisterService(service =>
                    {
                        service.RegisterTypeFactory(factory => factory.SetDefaultConstructor<WorldObjectSlaveCRUDComponent>());
                    });
                });

            services.RegisterEntity<AetherBodyWorldObject>()
                .RegisterComponent<AetherBodyWorldObjectMasterValidateWorldInfoChangeDetectedComponent>(component =>
                {
                    component.SetAssignableEntityType<AetherBodyWorldObject>()
                        .RegisterService(service =>
                        {
                            service.RegisterTypeFactory(factory => factory.SetDefaultConstructor<AetherBodyWorldObjectMasterValidateWorldInfoChangeDetectedComponent>());
                        });
                })
                .RegisterComponent<AetherWorldObjectRemoteMasterComponent>(component =>
                {
                    component.SetAssignableEntityType<AetherBodyWorldObject>()
                        .RegisterService(service =>
                        {
                            service.RegisterTypeFactory(factory => factory.SetDefaultConstructor<AetherWorldObjectRemoteMasterComponent>());
                        });
                })
                .RegisterComponent<AetherWorldObjectRemoteSlaveComponent>(component =>
                {
                    component.SetAssignableEntityType<AetherBodyWorldObject>()
                        .RegisterService(service =>
                        {
                            service.RegisterTypeFactory(factory => factory.SetDefaultConstructor<AetherWorldObjectRemoteSlaveComponent>());
                        });
                });                
        }
    }
}
