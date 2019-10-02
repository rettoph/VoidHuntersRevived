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
using GalacticFighters.Library.Configurations;
using GalacticFighters.Library.Entities.ShipParts.Thrusters;

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

            provider.GetRequiredService<GlobalOptions>().LogLevel = LogLevel.Information;

            var entities = provider.GetRequiredService<EntityLoader>();
            var builder = new ShipPartConfigurationBuilder();

            // Register players
            entities.TryRegister<UserPlayer>("player:user");

            // Register misc entities
            entities.TryRegister<Ship>("ship");
            entities.TryRegister<TractorBeam>("tractor-beam");

            #region Register Hull Pieces
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
            #endregion

            #region Register Chassis
            // Create mosquito chassis
            builder.AddNode(Vector2.Zero, 0, ShipPartConfigurationBuilder.NodeType.Male);
            builder.AddSide(MathHelper.ToRadians(180), ShipPartConfigurationBuilder.NodeType.None);
            builder.AddSide(MathHelper.ToRadians(120), ShipPartConfigurationBuilder.NodeType.Female);
            builder.AddSide(MathHelper.ToRadians(120), ShipPartConfigurationBuilder.NodeType.Female);
            builder.AddSide(MathHelper.ToRadians(120), ShipPartConfigurationBuilder.NodeType.Female);
            builder.AddSide(MathHelper.ToRadians(180), ShipPartConfigurationBuilder.NodeType.Female);
            builder.AddSide(MathHelper.ToRadians(120), ShipPartConfigurationBuilder.NodeType.Female);
            builder.AddSide(MathHelper.ToRadians(120), ShipPartConfigurationBuilder.NodeType.Female);
            builder.AddSide(MathHelper.ToRadians(120), ShipPartConfigurationBuilder.NodeType.None);
            builder.Rotate(MathHelper.ToRadians(90));
            builder.Flush();
            builder.AddSide(MathHelper.ToRadians(180), ShipPartConfigurationBuilder.NodeType.Female);
            builder.AddSide(MathHelper.ToRadians(150), ShipPartConfigurationBuilder.NodeType.Female);
            builder.AddSide(MathHelper.ToRadians(120), ShipPartConfigurationBuilder.NodeType.Female);
            builder.AddSide(MathHelper.ToRadians(120), ShipPartConfigurationBuilder.NodeType.Female);
            builder.AddSide(MathHelper.ToRadians(150), ShipPartConfigurationBuilder.NodeType.Female);
            builder.Transform(Matrix.CreateTranslation(0, -1, 0));
            entities.TryRegister<RigidShipPart>(
                "ship-part:chassis:mosquito",
                "name:entity:ship-part:chassis:mosquito",
                "description:entity:ship-part:chassis",
                builder.Build());
            #endregion

            #region Register Thrusters
            entities.TryRegister<Thruster>(
                "ship-part:thruster:small",
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
                    }));
            #endregion
        }
    }
}
