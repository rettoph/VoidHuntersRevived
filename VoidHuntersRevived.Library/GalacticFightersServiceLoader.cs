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
using GalacticFighters.Library.Configurations;
using GalacticFighters.Library.Drivers;
using GalacticFighters.Library.Entities;
using GalacticFighters.Library.Entities.Players;
using GalacticFighters.Library.Entities.ShipParts;
using GalacticFighters.Library.Entities.ShipParts.Hulls;
using GalacticFighters.Library.Entities.ShipParts.Thrusters;
using GalacticFighters.Library.Scenes;
using GalacticFighters.Library.Utilities;
using static GalacticFighters.Library.Utilities.ShipPartConfigurationBuilder;
using Guppy.Attributes;

namespace GalacticFighters.Library
{
    [IsServiceLoader]
    internal sealed class GalacticFightersServiceLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // services.AddScoped<ShipBuilder>();
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
            Settings.MaxPolygonVertices = 25;
            Settings.UseConvexHullPolygons = true;

            var stringLoader = provider.GetRequiredService<StringLoader>();
            stringLoader.TryRegister("description:entity:ship-part:hull", "A hull piece that can be used increase your ship size.");
            stringLoader.TryRegister("name:entity:ship-part:hull:square", "Hull Square");

            var entityLoader = provider.GetRequiredService<EntityLoader>();

            var builder = new ShipPartConfigurationBuilder();
            builder.AddNode(Vector2.Zero, 0, NodeType.Male);
            builder.AddSide(MathHelper.ToRadians(180), NodeType.None);
            builder.AddSide(MathHelper.ToRadians(120), NodeType.Female);
            builder.AddSide(MathHelper.ToRadians(120), NodeType.Female);
            builder.AddSide(MathHelper.ToRadians(120), NodeType.Female);
            builder.AddSide(MathHelper.ToRadians(180), NodeType.Female);
            builder.AddSide(MathHelper.ToRadians(120), NodeType.Female);
            builder.AddSide(MathHelper.ToRadians(120), NodeType.Female);
            builder.AddSide(MathHelper.ToRadians(120), NodeType.None);
            builder.Rotate(MathHelper.ToRadians(90));
            builder.Flush();
            builder.AddSide(MathHelper.ToRadians(180), NodeType.Female);
            builder.AddSide(MathHelper.ToRadians(150), NodeType.Female);
            builder.AddSide(MathHelper.ToRadians(120), NodeType.Female);
            builder.AddSide(MathHelper.ToRadians(120), NodeType.Female);
            builder.AddSide(MathHelper.ToRadians(150), NodeType.Female);
            builder.Transform(Matrix.CreateTranslation(0, -1, 0));
            entityLoader.TryRegister<Hull>(
                "entity:ship-part:chassis:mosquito",
                "name:entity:ship-part:chassis:mosquito",
                "description:entity:ship-part:chassis",
                builder.Build());

            builder.Clear();
            builder.AddSide(MathHelper.ToRadians(180), NodeType.Male);
            builder.AddSide(MathHelper.ToRadians(90), NodeType.Female);
            builder.AddSide(MathHelper.ToRadians(150), NodeType.Female);
            builder.AddSide(MathHelper.ToRadians(60), NodeType.Female);
            builder.AddSide(MathHelper.ToRadians(150), NodeType.Female);
            entityLoader.TryRegister<Hull>(
                "entity:ship-part:hull:pentagon",
                "name:entity:ship-part:hull:pentagon",
                "description:entity:ship-part:hull",
                builder.Build());

            entityLoader.TryRegister<Hull>(
                "entity:ship-part:hull:triangle",
                "name:entity:ship-part:hull:triangle",
                "description:entity:ship-part:hull",
                ShipPartConfigurationBuilder.BuildPolygon(3, true));
            entityLoader.TryRegister<Hull>(
                "entity:ship-part:hull:square",
                "name:entity:ship-part:hull:square",
                "description:entity:ship-part:hull",
                ShipPartConfigurationBuilder.BuildPolygon(4, true));
            entityLoader.TryRegister<Hull>(
                "entity:ship-part:hull:hexagon",
                "name:entity:ship-part:hull:hexagon",
                "description:entity:ship-part:hull",
                ShipPartConfigurationBuilder.BuildPolygon(6, true));
            entityLoader.TryRegister<Thruster>(
                "entity:ship-part:thruster:small",
                "name:entity:ship-part:thruster:small",
                "description:entity:ship-part:thruster",
                new ShipPartConfiguration(
                    vertices: new Vertices(
                        new Vector2[] {
                            new Vector2(-0.1f, -0.3f),
                            new Vector2(-0.1f, 0.3f),
                            new Vector2(0.4f, 0.1f),
                            new Vector2(0.4f, -0.1f)
                        }),
                    maleConnectionNode: new ConnectionNodeConfiguration()
                    {
                        Position = new Vector2(0.3f, 0),
                        Rotation = 0
                    },
                    density: 0.05f));

            entityLoader.TryRegister<TractorBeam>(
                "entity:tractor-beam", 
                "name:entity:tractor-beam", 
                "description:entity:tractor-beam");
            entityLoader.TryRegister<Ship>(
                "entity:ship",
                "name:entity:ship",
                "description:entity:ship");
            entityLoader.TryRegister<UserPlayer>(
                "entity:player:user",
                "name:entity:player:user",
                "description:entity:player:user");

            var randomTypeLoader = provider.GetRequiredService<RandomStringLoader>();
            randomTypeLoader.TryRegister("ship-part:bridge", "entity:ship-part:chassis:mosquito");

            randomTypeLoader.TryRegister("ship-part:hull", "entity:ship-part:hull:pentagon");
            randomTypeLoader.TryRegister("ship-part:hull", "entity:ship-part:hull:square");
            randomTypeLoader.TryRegister("ship-part:hull", "entity:ship-part:hull:triangle");
            randomTypeLoader.TryRegister("ship-part:hull", "entity:ship-part:hull:hexagon");

            randomTypeLoader.TryRegister("ship-part:thruster", "entity:ship-part:thruster:small");
        }
    }
}
