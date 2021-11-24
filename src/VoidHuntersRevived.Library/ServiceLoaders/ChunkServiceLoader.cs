using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Interfaces;
using Guppy.Lists;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Chunks;
using VoidHuntersRevived.Library.Components.Entities.Chunks;

namespace VoidHuntersRevived.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class ChunkServiceLoader : IServiceLoader
    {
        public void RegisterServices(AssemblyHelper assemblyHelper, GuppyServiceCollection services)
        {
            services.RegisterTypeFactory<Chunk>(p => new Chunk(), 5000);
            services.RegisterTypeFactory<ChunkManager>(p => new ChunkManager());
            services.RegisterTypeFactory<FrameableList<Chunk>>(p => new FrameableList<Chunk>());

            services.RegisterTransient<Chunk>();
            services.RegisterScoped<ChunkManager>();
            services.RegisterScoped<FrameableList<Chunk>>();

            #region Components
            services.RegisterTypeFactory<ChunkPipeComponent>(p => new ChunkPipeComponent());

            services.RegisterTransient<ChunkPipeComponent>();

            services.RegisterComponent<ChunkPipeComponent, Chunk>();
            #endregion
        }

        public void ConfigureProvider(GuppyServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
