using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Components.Entities.Chunks;
using VoidHuntersRevived.Library.Entities.Chunks;

namespace VoidHuntersRevived.Client.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class ChunkServiceLoader : IServiceLoader
    {
        public void RegisterServices(GuppyServiceCollection services)
        {
            services.RegisterTypeFactory<ChunkDrawComponent>(p => new ChunkDrawComponent());

            services.RegisterTransient<ChunkDrawComponent>();

            services.RegisterComponent<ChunkDrawComponent, Chunk>();
        }

        public void ConfigureProvider(GuppyServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
