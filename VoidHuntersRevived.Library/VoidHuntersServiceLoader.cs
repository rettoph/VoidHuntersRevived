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
using VoidHuntersRevived.Library.Collections;
using VoidHuntersRevived.Library.Configurations;
using VoidHuntersRevived.Library.Drivers;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Library
{
    public class VoidHuntersServiceLoader : IServiceLoader
    {
        public void ConfigureServiceCollection(IServiceCollection services)
        {
            services.AddScene<VoidHuntersWorldScene>();
            services.AddDriver<VoidHuntersWorldScene, VoidHuntersWorldSceneDriver>(95);

            services.AddScoped<ShipCollection>();
            services.AddScoped<World>(p =>
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
            entityLoader.Register<RigidShipPart>(
                "entity:ship-part", 
                "name:entity:ship-part", 
                "description:entity:ship-part",
                new ShipPartConfiguration(
                    vertices: new Vertices(
                        new Vector2[] {
                            new Vector2(-0.5f, -0.5f),
                            new Vector2(-0.5f, 0.5f),
                            new Vector2(0.5f, 0.5f),
                            new Vector2(0.5f, -0.5f)
                        }),
                    maleConnectionNode: new Vector3(-0.5f, 0, (Single)Math.PI),
                    femaleConnectionNodes: new Vector3[] {
                        new Vector3(0, -0.5f, (Single)Math.PI/2),
                        new Vector3(0.5f, 0, (Single)Math.PI),
                        new Vector3(0, 0.5f, -(Single)Math.PI/2)
                    }));
            entityLoader.Register<TractorBeam>(
                "entity:tractor-beam", 
                "name:entity:tractor-beam", 
                "description:entity:tractor-beam");
            entityLoader.Register<Ship>(
                "entity:ship",
                "name:entity:ship",
                "description:entity:ship");
            entityLoader.Register<UserPlayer>(
                "entity:player:user",
                "name:entity:player:user",
                "description:entity:player:user");
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
