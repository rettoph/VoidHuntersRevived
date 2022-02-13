using Guppy.Attributes;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.EntityComponent.DependencyInjection.Builders;
using Guppy.Network.Builders;
using Guppy.ServiceLoaders;
using LiteNetLib;
using System;
using VoidHuntersRevived.Library.Components.Ships;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.MessageProcessors;
using VoidHuntersRevived.Library.Messages.Network;
using VoidHuntersRevived.Library.Messages.Network.Packets;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class ShipServiceLoader : IServiceLoader, INetworkLoader
    {
        public void ConfigureNetwork(NetworkProviderBuilder network)
        {
            network.RegisterNetworkEntityMessage<ShipDirectionChangedMessage>()
                .SetDeliveryMethod(DeliveryMethod.ReliableSequenced);

            network.RegisterNetworkEntityMessage<ShipTargetMessage>()
                .SetDeliveryMethod(DeliveryMethod.Unreliable);

            network.RegisterNetworkEntityMessage<ShipTractorBeamStateChangedMessage>()
                .SetDeliveryMethod(DeliveryMethod.ReliableOrdered);

            network.RegisterNetworkEntityMessage<ShipPlayerChangedMessage>()
                .SetDeliveryMethod(DeliveryMethod.ReliableOrdered);

            network.RegisterDataType<ShipCreatePacket>()
                .SetReader(ShipCreatePacket.Read)
                .SetWriter(ShipCreatePacket.Write);
        }

        public void RegisterServices(AssemblyHelper assemblyHelper, ServiceProviderBuilder services)
        {
            services.RegisterService<ShipService>()
                .SetLifetime(ServiceLifetime.Scoped)
                .RegisterTypeFactory(factory => factory.SetDefaultConstructor<ShipService>());

            services.RegisterService<TractorBeamRequestProcessor>()
                .SetLifetime(ServiceLifetime.Scoped)
                .RegisterTypeFactory(factory => factory.SetDefaultConstructor<TractorBeamRequestProcessor>());

            services.RegisterService<DirectionRequestProcessor>()
                .SetLifetime(ServiceLifetime.Scoped)
                .RegisterTypeFactory(factory => factory.SetDefaultConstructor<DirectionRequestProcessor>());

            services.RegisterEntity<Ship>()
                .RegisterService(service =>
                {
                    service.SetLifetime(ServiceLifetime.Transient)
                        .RegisterTypeFactory(factory => factory.SetDefaultConstructor<Ship>());
                })
                .RegisterComponent<ShipMasterCRUDComponent>(component =>
                {
                    component.RegisterService(service =>
                    {
                        service.RegisterTypeFactory(factory =>
                        {
                            factory.SetDefaultConstructor<ShipMasterCRUDComponent>();
                        });
                    });
                })
                .RegisterComponent<ShipSlaveCRUDComponent>(component =>
                {
                    component.RegisterService(service =>
                    {
                        service.RegisterTypeFactory(factory =>
                        {
                            factory.SetDefaultConstructor<ShipSlaveCRUDComponent>();
                        });
                    });
                })
                .RegisterComponent<TargetComponent>(component =>
                {
                    component.RegisterService(service =>
                    {
                        service.RegisterTypeFactory(factory =>
                        {
                            factory.SetDefaultConstructor<TargetComponent>();
                        });
                    });
                })
                .RegisterComponent<TargetRemoteMasterComponent>(component =>
                {
                    component.RegisterService(service =>
                    {
                        service.RegisterTypeFactory(factory =>
                        {
                            factory.SetDefaultConstructor<TargetRemoteMasterComponent>();
                        });
                    });
                })
                .RegisterComponent<TargetRemoteSlaveComponent>(component =>
                {
                    component.RegisterService(service =>
                    {
                        service.RegisterTypeFactory(factory =>
                        {
                            factory.SetDefaultConstructor<TargetRemoteSlaveComponent>();
                        });
                    });
                })
                .RegisterComponent<DirectionComponent>(component =>
                {
                    component.RegisterService(service =>
                    {
                        service.RegisterTypeFactory(factory =>
                        {
                            factory.SetDefaultConstructor<DirectionComponent>();
                        });
                    });
                })
                .RegisterComponent<DirectionRemoteMasterComponent>(component =>
                {
                    component.RegisterService(service =>
                    {
                        service.RegisterTypeFactory(factory =>
                        {
                            factory.SetDefaultConstructor<DirectionRemoteMasterComponent>();
                        });
                    });
                })
                .RegisterComponent<DirectionRemoteSlaveComponent>(component =>
                {
                    component.RegisterService(service =>
                    {
                        service.RegisterTypeFactory(factory =>
                        {
                            factory.SetDefaultConstructor<DirectionRemoteSlaveComponent>();
                        });
                    });
                })
                .RegisterComponent<TractorBeamComponent>(component =>
                {
                    component.RegisterService(service =>
                    {
                        service.RegisterTypeFactory(factory =>
                        {
                            factory.SetDefaultConstructor<TractorBeamComponent>();
                        });
                    });
                })
                .RegisterComponent<TractorBeamRemoteMasterComponent>(component =>
                {
                    component.RegisterService(service =>
                    {
                        service.RegisterTypeFactory(factory =>
                        {
                            factory.SetDefaultConstructor<TractorBeamRemoteMasterComponent>();
                        });
                    });
                })
                .RegisterComponent<TractorBeamRemoteSlaveComponent>(component =>
                {
                    component.RegisterService(service =>
                    {
                        service.RegisterTypeFactory(factory =>
                        {
                            factory.SetDefaultConstructor<TractorBeamRemoteSlaveComponent>();
                        });
                    });
                });
                // .RegisterComponent<ShipThrustersSlaveCRUDComponent>(component =>
                // {
                //     component.RegisterService(service =>
                //     {
                //         service.RegisterTypeFactory(factory =>
                //         {
                //             factory.SetDefaultConstructor<ShipThrustersSlaveCRUDComponent>();
                //         });
                //     });
                // })
                // .RegisterComponent<ShipTargetingMasterCRUDComponent>(component =>
                // {
                //     component.RegisterService(service =>
                //     {
                //         service.RegisterTypeFactory(factory =>
                //         {
                //             factory.SetDefaultConstructor<ShipTargetingMasterCRUDComponent>();
                //         });
                //     });
                // })
                // .RegisterComponent<ShipTargetingSlaveCrudComponent>(component =>
                // {
                //     component.RegisterService(service =>
                //     {
                //         service.RegisterTypeFactory(factory =>
                //         {
                //             factory.SetDefaultConstructor<ShipTargetingSlaveCrudComponent>();
                //         });
                //     });
                // });
                // .RegisterComponent<ShipTractorBeamMasterCRUDComponent>(component =>
                // {
                //     component.RegisterService(service =>
                //     {
                //         service.RegisterTypeFactory(factory =>
                //         {
                //             factory.SetDefaultConstructor<ShipTractorBeamMasterCRUDComponent>();
                //         });
                //     });
                // })
                // .RegisterComponent<ShipTractorBeamSlaveCRUDComponent>(component =>
                // {
                //     component.RegisterService(service =>
                //     {
                //         service.RegisterTypeFactory(factory =>
                //         {
                //             factory.SetDefaultConstructor<ShipTractorBeamSlaveCRUDComponent>();
                //         });
                //     });
                // });
        }
    }
}
