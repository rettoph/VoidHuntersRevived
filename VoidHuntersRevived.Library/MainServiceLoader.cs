using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Guppy.Attributes;
using Guppy.Collections;
using Guppy.Factories;
using Guppy.Interfaces;
using Guppy.Loaders;
using Guppy.Utilities.Options;
using Lidgren.Network;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Collections;
using VoidHuntersRevived.Library.Configurations;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Ammunitions;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.Hulls;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;
using VoidHuntersRevived.Library.Entities.ShipParts.Weapons;
using VoidHuntersRevived.Library.Utilities;
using VoidHuntersRevived.Library.Utilities.Delegaters;
using Guppy.Extensions.DependencyInjection;

namespace VoidHuntersRevived.Library
{
    [IsServiceLoader]
    internal sealed class MainServiceLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            Settings.MaxPolygonVertices = 16;

            services.AddScoped<Quarantine>("entity:quarantine");
            services.AddScoped<Annex>("entity:annex");
            services.AddScoped<World>(p => new World(Vector2.Zero));
            services.AddScoped<ShipBuilder>();
            services.AddScoped<ChunkCollection>();
            services.AddScoped<List<Player>>();

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
            provider.GetRequiredService<GlobalOptions>().LogLevel = Microsoft.Extensions.Logging.LogLevel.Debug;

            var entities = provider.GetRequiredService<EntityLoader>();

            entities.TryRegister<UserPlayer>("entity:player:user", "name:entity:player:user", "description:entity:player:user");
            entities.TryRegister<ComputerPlayer>("entity:player:computer", "name:entity:player:computer", "description:entity:player:computer");

            entities.TryRegister<Quarantine>("entity:quarantine", "name:entity:quarantine", "description:entity:quarantine");
            entities.TryRegister<Annex>("entity:annex", "name:entity:annex", "description:entity:annex");
            entities.TryRegister<Explosion>("entity:explosion", "name:entity:explosion", "description:entity:explosion");
            entities.TryRegister<Chunk>("entity:chunk", "name:entity:chunk", "description:entity:chunk");
            entities.TryRegister<CustomController>("entity:custom-controller", "name:entity:custom-controller", "description:entity:custom-controller");

            entities.TryRegister<Ship>("entity:ship", "name:entity:ship", "description:entity:ship");
            entities.TryRegister<TractorBeam>("entity:tractor-beam", "name:entity:tractor-beam", "description:entity:tractor-beam");

            #region Register ShipParts
            #region Register Hulls

            #region Triangle
            var triangle = new ShipPartConfiguration();
            triangle.AddPolygon(3);
            triangle.Flush();

            entities.TryRegister<Hull>(
                handle: "entity:ship-part:hull:triangle",
                nameHandle: "name:entity:ship-part:hull:triangle", 
                descriptionHandle: "description:entity:ship-part:hull:triangle", 
                data: triangle);
            #endregion

            #region Square
            var square = new ShipPartConfiguration();
            square.AddPolygon(4);
            square.Flush();

            entities.TryRegister<Hull>(
                handle: "entity:ship-part:hull:square",
                nameHandle: "name:entity:ship-part:hull:square",
                descriptionHandle: "description:entity:ship-part:hull:square",
                data: square);
            #endregion

            #region Hexagon
            var hexagon = new ShipPartConfiguration();
            hexagon.AddPolygon(6);
            hexagon.Flush();

            entities.TryRegister<Hull>(
                handle: "entity:ship-part:hull:hexagon",
                nameHandle: "name:entity:ship-part:hull:hexagon",
                descriptionHandle: "description:entity:ship-part:hull:hexagon",
                data: hexagon);
            #endregion

            #endregion

            #region Chassis

            #region Mosquito
            // Create mosquito chassis
            var mosquito = new ShipPartConfiguration();
            mosquito.AddNode(Vector2.Zero, 0, ShipPartConfiguration.NodeType.Male);
            mosquito.AddSide(MathHelper.ToRadians(180), ShipPartConfiguration.NodeType.None);
            mosquito.AddSide(MathHelper.ToRadians(120), ShipPartConfiguration.NodeType.Female);
            mosquito.AddSide(MathHelper.ToRadians(120), ShipPartConfiguration.NodeType.Female);
            mosquito.AddSide(MathHelper.ToRadians(120), ShipPartConfiguration.NodeType.Female);
            mosquito.AddSide(MathHelper.ToRadians(180), ShipPartConfiguration.NodeType.Female);
            mosquito.AddSide(MathHelper.ToRadians(120), ShipPartConfiguration.NodeType.Female);
            mosquito.AddSide(MathHelper.ToRadians(120), ShipPartConfiguration.NodeType.Female);
            mosquito.AddSide(MathHelper.ToRadians(120), ShipPartConfiguration.NodeType.None);
            mosquito.Rotate(MathHelper.ToRadians(90));
            mosquito.Flush();
            mosquito.AddSide(MathHelper.ToRadians(180), ShipPartConfiguration.NodeType.Female);
            mosquito.AddSide(MathHelper.ToRadians(150), ShipPartConfiguration.NodeType.Female);
            mosquito.AddSide(MathHelper.ToRadians(120), ShipPartConfiguration.NodeType.Female);
            mosquito.AddSide(MathHelper.ToRadians(120), ShipPartConfiguration.NodeType.Female);
            mosquito.AddSide(MathHelper.ToRadians(150), ShipPartConfiguration.NodeType.Female);
            mosquito.Transform(Matrix.CreateTranslation(0, -1, 0));
            mosquito.Flush();
            entities.TryRegister<Hull>(
                "entity:ship-part:chassis:mosquito",
                "name:entity:ship-part:chassis:mosquito",
                "description:entity:ship-part:chassis",
                mosquito);
            #endregion

            #endregion

            #region Thrusters
            var thruster = new ShipPartConfiguration();
            thruster.AddVertice(-0.1f, -0.3f);
            thruster.AddVertice(-0.1f, 0.3f);
            thruster.AddVertice(0.4f, 0.1f);
            thruster.AddVertice(0.4f, -0.1f);
            thruster.AddNode(0.3f, 0, 0, ShipPartConfiguration.NodeType.Male);
            thruster.Flush();

            entities.TryRegister<Thruster>(
                handle: "entity:ship-part:thruster:small",
                nameHandle: "name:entity:ship-part:thruster:small",
                descriptionHandle: "description:entity:ship-part:thruster:small",
                data: thruster);
            #endregion

            #region Weapons
            var massDriver = new GunConfiguration();
            massDriver.SetSwivelRange(MathHelper.PiOver2);
            massDriver.SetFireRate(250);
            massDriver.SetFireStrength(10f);
            massDriver.SetProjectileHandle("entity:ammunition:projectile:mass-driver");
            massDriver.AddVertice(0f, -0.15f);
            massDriver.AddVertice(0f, 0.15f);
            massDriver.AddVertice(0.6f, 0.075f);
            massDriver.AddVertice(0.6f, -0.075f);
            massDriver.AddNode(0.15f, 0, MathHelper.Pi, ShipPartConfiguration.NodeType.Male);
            massDriver.Flush();

            entities.TryRegister<Gun>(
                handle: "entity:ship-part:weapon:mass-driver",
                nameHandle: "name:entity:ship-part:weapon:mass-driver",
                descriptionHandle: "description:entity:ship-part:weapon:mass-driver",
                data: massDriver);
            #endregion
            #endregion

            #region Register Ammunitions
            entities.TryRegister<Projectile>(
                handle: "entity:ammunition:projectile:mass-driver",
                nameHandle: "name:entity:ammunition:projectile:mass-driver", 
                descriptionHandle: "description:entity:ammunition:projectile:mass-driver",
                data: new ProjectileConfiguration()
                {
                    MaxAge = 5000
                });
            #endregion
        }
    }
}
