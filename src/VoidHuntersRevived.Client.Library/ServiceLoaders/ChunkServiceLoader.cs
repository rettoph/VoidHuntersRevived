using Guppy.Attributes;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.EntityComponent.DependencyInjection.Builders;
using Guppy.Interfaces;
using Guppy.ServiceLoaders;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Components.Chunks;
using VoidHuntersRevived.Library.Entities.Chunks;

namespace VoidHuntersRevived.Client.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class ChunkServiceLoader : IServiceLoader
    {
        public void RegisterServices(AssemblyHelper assemblyHelper, ServiceProviderBuilder services)
        {
            services.RegisterComponent<ChunkDrawComponent>()
                .SetAssignableEntityType<Chunk>()
                .RegisterService(service =>
                {
                    service.RegisterTypeFactory(factory =>
                    {
                        factory.SetDefaultConstructor<ChunkDrawComponent>();
                    });
                });
        }
    }
}
