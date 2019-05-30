using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics;
using Guppy.Extensions.DependencyInjection;
using Guppy.Interfaces;
using Guppy.Loaders;
using Lidgren.Network;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Configurations;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Library
{
    public class VoidHuntersServiceLoader : IServiceLoader
    {
        public void ConfigureServiceCollection(IServiceCollection services)
        {
            services.AddScene<VoidHuntersWorldScene>();

            services.AddTransient<World>(p =>
            {
                return new World(Vector2.Zero);
            });

            services.AddSingleton<NetPeerConfiguration>(p =>
            {
                var config = new NetPeerConfiguration("vhr")
                {
                    ConnectionTimeout = 5
                };

                // Enable required message types...
                config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
                

                return config;
            });
        }

        public void Boot(IServiceProvider provider)
        {
            var entityLoader = provider.GetLoader<EntityLoader>();
            entityLoader.Register<Player>("entity:player", "name:entity:player", "description:entity:player");
            entityLoader.Register<TractorBeam>("entity:tractor-beam", "name:entity:tractor-beam", "description:entity:tractor-beam");
            entityLoader.Register<ShipPart>(
                "entity:ship-part", 
                "name:entity:ship-part", 
                "description:entity:ship-part",
                new ShipPartConfiguration(
                    new PolygonShape(
                        new Vertices(
                            new Vector2[] {
                                new Vector2(0, 0),
                                new Vector2(0, 1),
                                new Vector2(1, 1),
                                new Vector2(1, 0)
                            }), 1f)));
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
