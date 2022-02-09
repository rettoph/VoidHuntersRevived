using Guppy.Attributes;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.Interfaces;
using Guppy.EntityComponent.Lists;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Chunks;
using VoidHuntersRevived.Library.Components.Chunks;
using Guppy.EntityComponent.DependencyInjection.Builders;
using Guppy.ServiceLoaders;

namespace VoidHuntersRevived.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class ChunkServiceLoader : IServiceLoader
    {
        public void RegisterServices(AssemblyHelper assemblyHelper, ServiceProviderBuilder services)
        {
            services.RegisterEntity<Chunk>()
                .RegisterService(service =>
                {
                    service.SetLifetime(ServiceLifetime.Transient)
                        .RegisterTypeFactory(factory => factory.SetDefaultConstructor<Chunk>());
                })
                .RegisterComponent<ChunkPipeComponent>(component =>
                {
                    component.RegisterService(service =>
                    {
                        service.RegisterTypeFactory(factory => factory.SetDefaultConstructor<ChunkPipeComponent>());
                    });
                });

            services.RegisterEntity<ChunkManager>()
                .RegisterService(service =>
                {
                    service.SetLifetime(ServiceLifetime.Scoped)
                        .RegisterTypeFactory(factory => factory.SetDefaultConstructor<ChunkManager>());
                })
                .RegisterComponent<ChunkManagerMasterPopulationComponent>(component =>
                {
                    component.RegisterService(service =>
                    {
                        service.RegisterTypeFactory(factory => factory.SetDefaultConstructor<ChunkManagerMasterPopulationComponent>());
                    });
                });

            services.RegisterService<FrameableList<Chunk>>()
                .SetLifetime(ServiceLifetime.Scoped)
                .RegisterTypeFactory(factory => factory.SetDefaultConstructor<FrameableList<Chunk>>());
        }
    }
}
