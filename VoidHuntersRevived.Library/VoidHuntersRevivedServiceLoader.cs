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
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Scenes;
using Guppy.Attributes;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Utilities;
using VoidHuntersRevived.Library.Factories;
using VoidHuntersRevived.Library.Entities.Players;
using Guppy.Utilities.Options;
using Microsoft.Extensions.Logging;
using VoidHuntersRevived.Library.Configurations;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;
using VoidHuntersRevived.Library.Entities.ShipParts.Weapons;
using VoidHuntersRevived.Library.Entities.ShipParts.Hulls;
using VoidHuntersRevived.Library.Entities.Ammunitions;
using VoidHuntersRevived.Library.Utilities.Delegater;

namespace VoidHuntersRevived.Library
{
    [IsServiceLoader]
    internal sealed class VoidHuntersRevivedServiceLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<Interval>();
            services.AddScoped<ConnectionNodeFactory>();
            services.AddScoped<Random>();
            services.AddScoped<ShipBuilder>();

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
                config.UseMessageRecycling = true;


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
            entities.TryRegister<ComputerPlayer>("player:computer");

            // Register misc entities
            entities.TryRegister<Ship>("ship");
            entities.TryRegister<TractorBeam>("tractor-beam");

            #region Register Hull Pieces
            // Register ShipParts
            entities.TryRegister<Hull>("ship-part:triangle", "", "", ShipPartConfiguration.BuildPolygon("sprite:ship-part:hull:triangle", 3, true));
            entities.TryRegister<Hull>("ship-part:square", "", "", ShipPartConfiguration.BuildPolygon("sprite:ship-part:hull:square", 4, true));
            entities.TryRegister<Hull>("ship-part:hexagon", "", "", ShipPartConfiguration.BuildPolygon("sprite:ship-part:hull:hexagon", 6, true));

            // Create the pentagon
            var config = new ShipPartConfiguration();
            config.Texture = "sprite:ship-part:hull:pentagon";
            config.AddSide(0, ShipPartConfiguration.NodeType.Both);
            config.AddSide(MathHelper.PiOver2, ShipPartConfiguration.NodeType.Female);
            config.AddSide((MathHelper.Pi / 3) + MathHelper.PiOver2, ShipPartConfiguration.NodeType.Female);
            config.AddSide(MathHelper.Pi / 3, ShipPartConfiguration.NodeType.Female);
            config.AddSide((MathHelper.Pi / 3) + MathHelper.PiOver2, ShipPartConfiguration.NodeType.Female);
            entities.TryRegister<Hull>("ship-part:pentagon", "", "", config.Flush());

            // Create the horizontal beam
            config = new ShipPartConfiguration();
            config.Texture = "sprite:ship-part:hull:beam:horizontal";
            config.AddSide(0, ShipPartConfiguration.NodeType.Female);
            config.AddSide(MathHelper.Pi, ShipPartConfiguration.NodeType.Both);
            config.AddSide(MathHelper.Pi, ShipPartConfiguration.NodeType.Female);
            config.AddSide(MathHelper.Pi / 2, ShipPartConfiguration.NodeType.Female);
            config.AddSide(MathHelper.Pi / 2, ShipPartConfiguration.NodeType.Female);
            config.AddSide(MathHelper.Pi, ShipPartConfiguration.NodeType.Female);
            config.AddSide(MathHelper.Pi, ShipPartConfiguration.NodeType.Female);
            config.AddSide(MathHelper.Pi / 2, ShipPartConfiguration.NodeType.Female);
            entities.TryRegister<Hull>("ship-part:beam:horizontal", "", "", config.Flush());
            #endregion

            #region Register Chassis
            // Create mosquito chassis
            config = new ShipPartConfiguration();
            config.Texture = "sprite:ship-part:chassis:mosquito";
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
                fireRate: 250f,
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

            entities.TryRegister<Gun>(
                "ship-part:weapon:mass-driver",
                "name:entity:ship-part:weapon:mass-driver",
                "description:entity:ship-part:weapon:mass-driver",
                weaponConfig.Flush());
            #endregion

            #region Register Ammo
            entities.TryRegister<Projectile>("bullet");
            #endregion
        }
    }
}
