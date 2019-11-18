using FarseerPhysics.Dynamics;
using Guppy.Attributes;
using Guppy.Collections;
using Guppy.Interfaces;
using Guppy.Loaders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Collections;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Utilities.Delegaters;

namespace VoidHuntersRevived.Library
{
    [IsServiceLoader]
    internal sealed class MainServiceLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ActionMessageDelegater>();
            services.AddScoped<World>(p => new World(Vector2.Zero));
            services.AddScoped<ChunkCollection>();
        }

        public void ConfigureProvider(IServiceProvider provider)
        {
            var entities = provider.GetRequiredService<EntityLoader>();

            entities.TryRegister<Chunk>("entity:chunk", "name:entity:chunk", "description:entity:chunk");

            entities.TryRegister<ShipPart>("entity:ship-part", "name:entity:ship-part", "description:entity:ship-part");
        }
    }
}
