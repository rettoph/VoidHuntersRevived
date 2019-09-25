using FarseerPhysics;
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
using GalacticFighters.Library.Entities;
using GalacticFighters.Library.Scenes;
using Guppy.Attributes;
using GalacticFighters.Library.Entities.ShipParts;
using GalacticFighters.Library.Utilities;
using GalacticFighters.Library.Factories;
using GalacticFighters.Library.Entities.Players;
using Guppy.Utilities.Options;
using Microsoft.Extensions.Logging;

namespace GalacticFighters.Library
{
    [IsServiceLoader]
    internal sealed class GalacticFightersServiceLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ConnectionNodeFactory>();
            services.AddScoped<Random>();

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

        public void ConfigureProvider(IServiceProvider provider)
        {
            Settings.MaxPolygonVertices = 16;

            provider.GetRequiredService<GlobalOptions>().LogLevel = LogLevel.Debug;

            var entities = provider.GetRequiredService<EntityLoader>();
            var builder = new ShipPartConfigurationBuilder();

            // Register players
            entities.TryRegister<UserPlayer>("player:user");

            // Register misc entities
            entities.TryRegister<Ship>("ship");
            entities.TryRegister<TractorBeam>("tractor-beam");

            // Register ShipParts
            entities.TryRegister<RigidShipPart>("ship-part:triangle", "", "", ShipPartConfigurationBuilder.BuildPolygon(3, true));
            entities.TryRegister<RigidShipPart>("ship-part:square", "", "", ShipPartConfigurationBuilder.BuildPolygon(4, true));
            entities.TryRegister<RigidShipPart>("ship-part:hexagon", "", "", ShipPartConfigurationBuilder.BuildPolygon(6, true));

            // Create the pentagon
            builder.AddSide(0, ShipPartConfigurationBuilder.NodeType.Male);
            builder.AddSide(MathHelper.PiOver2, ShipPartConfigurationBuilder.NodeType.Female);
            builder.AddSide((MathHelper.Pi / 3) + MathHelper.PiOver2, ShipPartConfigurationBuilder.NodeType.Female);
            builder.AddSide(MathHelper.Pi / 3, ShipPartConfigurationBuilder.NodeType.Female);
            builder.AddSide((MathHelper.Pi / 3) + MathHelper.PiOver2, ShipPartConfigurationBuilder.NodeType.Female);
            entities.TryRegister<RigidShipPart>("ship-part:pentagon", "", "", builder.Build());
        }
    }
}
