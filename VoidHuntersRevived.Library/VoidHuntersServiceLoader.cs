using FarseerPhysics.Dynamics;
using Guppy.Extensions;
using Guppy.Interfaces;
using Guppy.Loaders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library
{
    public class VoidHuntersServiceLoader : IServiceLoader
    {
        public void ConfigureServiceCollection(IServiceCollection services)
        {
            services.AddScoped<World>(p =>
            {
                return new World(Vector2.Zero);
            });
        }

        public void Boot(IServiceProvider provider)
        {
            var entityLoader = provider.GetLoader<EntityLoader>();

            entityLoader.Register<ShipPart>("entity:ship-part", "name:entity:ship-part", "description:entity:ship-part");
        }

        public void PreInitialize(IServiceProvider provider)
        {
            // throw new NotImplementedException();
        }

        public void Initialize(IServiceProvider provider)
        {
            // throw new NotImplementedException();
        }

        public void PostInitialize(IServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
