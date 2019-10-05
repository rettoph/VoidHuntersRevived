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
using GalacticFighters.Library.Entities.ShipParts.Weapons;
using GalacticFighters.Library.Entities.ShipParts.Hulls;

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

            // Register players
            entities.TryRegister<UserPlayer>("player:user");

            // Register misc entities
            entities.TryRegister<Ship>("ship");
            entities.TryRegister<TractorBeam>("tractor-beam");

            #region Register Hull Pieces
            // Register ShipParts
            entities.TryRegister<Hull>("ship-part:triangle", "", "", ShipPartConfiguration.BuildPolygon(3, true));
            entities.TryRegister<Hull>("ship-part:square", "", "", ShipPartConfiguration.BuildPolygon(4, true));
            entities.TryRegister<Hull>("ship-part:hexagon", "", "", ShipPartConfiguration.BuildPolygon(6, true));

            // Create the pentagon
            var config = new ShipPartConfiguration();
            config.AddSide(0, ShipPartConfiguration.NodeType.Male);
            config.AddSide(MathHelper.PiOver2, ShipPartConfiguration.NodeType.Female);
            config.AddSide((MathHelper.Pi / 3) + MathHelper.PiOver2, ShipPartConfiguration.NodeType.Female);
            config.AddSide(MathHelper.Pi / 3, ShipPartConfiguration.NodeType.Female);
            config.AddSide((MathHelper.Pi / 3) + MathHelper.PiOver2, ShipPartConfiguration.NodeType.Female);
            entities.TryRegister<Hull>("ship-part:pentagon", "", "", config.Flush());
            #endregion

            #region Register Chassis
            // Create mosquito chassis
            config = new ShipPartConfiguration();
            config.AddNode(Vector2.Zero, 0, ShipPartConfiguration.NodeType.Male);
            config.AddSide(MathHelper.ToRadians(180), ShipPartConfiguration.NodeType.None);
            config.AddSide(MathHelper.ToRadians(120), ShipPartConfiguration.NodeType.Female);
            config.AddSide(MathHelper.ToRadians(120), ShipPartConfiguration.NodeType.Female);
            config.AddSide(MathHelper.ToRadians(120), ShipPartConfiguration.NodeType.Female);
            config.AddSide(MathHelper.ToRadians(180), ShipPartConfiguration.NodeType.Female);
            config.AddSide(MathHelper.ToRadians(120), ShipPartConfiguration.NodeType.Female);
            config.AddSide(MathHelper.ToRadians(120), ShipPartConfiguration.NodeType.Female);
            config.AddSide(MathHelper.ToRadians(120), ShipPartConfiguration.NodeType.None);
            config.Rotate(MathHelper.ToRadians(90));
            config.Flush();
            config.AddSide(MathHelper.ToRadians(180), ShipPartConfiguration.NodeType.Female);
            config.AddSide(MathHelper.ToRadians(150), ShipPartConfiguration.NodeType.Female);
            config.AddSide(MathHelper.ToRadians(120), ShipPartConfiguration.NodeType.Female);
            config.AddSide(MathHelper.ToRadians(120), ShipPartConfiguration.NodeType.Female);
            config.AddSide(MathHelper.ToRadians(150), ShipPartConfiguration.NodeType.Female);
            config.Transform(Matrix.CreateTranslation(0, -1, 0));
            entities.TryRegister<Hull>(
                "ship-part:chassis:mosquito",
                "name:entity:ship-part:chassis:mosquito",
                "description:entity:ship-part:chassis",
                config.Flush());
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

            #region Register Weapons
            var weaponConfig = new WeaponConfiguration(
                vertices: new Vertices(
                    new Vector2[] {
                        new Vector2(-0.2f, -0.2f),
                        new Vector2(-0.2f, 0.2f),
                        new Vector2(0.2f, 0.2f),
                        new Vector2(0.2f, -0.2f)
                    }),
                maleConnectionNode: new ConnectionNodeConfiguration()
                {
                    Position = Vector2.Zero,
                    Rotation = 0
                });

            weaponConfig.AddBarrel(new Vertices(new Vector2[]
            {
                new Vector2(0f, -0.1f),
                new Vector2(0f, 0.1f),
                new Vector2(0.5f, 0.1f),
                new Vector2(0.5f, -0.1f)
            }), new Vector2(-0.2f, 0));

            entities.TryRegister<Weapon>(
                "ship-part:weapon:mass-driver",
                "name:entity:ship-part:weapon:mass-driver",
                "description:entity:ship-part:weapon:mass-driver",
                weaponConfig.Flush());
            #endregion
        }
    }
}
