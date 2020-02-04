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
using VoidHuntersRevived.Server.Utilities;

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
            services.AddScoped<ChunkCollection>();
            services.AddScoped<VitalsManager>();
            services.AddScoped<List<Player>>();
            services.AddScoped<List<Team>>();

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

            var strings = provider.GetRequiredService<StringLoader>();
            strings.TryRegister("name:entity:ship-part:hull:triangle", "Triangle Hull");
            strings.TryRegister("description:entity:ship-part:hull:triangle", "Useful for extending the size of your ship.");
            strings.TryRegister("name:entity:ship-part:hull:square", "Square Hull");
            strings.TryRegister("description:entity:ship-part:hull:square", "Useful for extending the size of your ship.");
            strings.TryRegister("name:entity:ship-part:hull:hexagon", "Hexagon Hull");
            strings.TryRegister("description:entity:ship-part:hull:pentagon", "Useful for extending the size of your ship.");
            strings.TryRegister("name:entity:ship-part:hull:pentagon", "Pentagon Hull");
            strings.TryRegister("description:entity:ship-part:hull:hexagon", "Useful for extending the size of your ship.");
            strings.TryRegister("name:entity:ship-part:chassis:mosquito", "Mosquito Chassis");
            strings.TryRegister("description:entity:ship-part:chassis:mosquito", "A Ship's mosquito chassis.");
            strings.TryRegister("name:entity:ship-part:thruster:small", "Thruster");
            strings.TryRegister("description:entity:ship-part:thruster:small", "Applies thrust to control your ship.");
            strings.TryRegister("name:entity:ship-part:weapon:mass-driver", "Mass Driver");
            strings.TryRegister("description:entity:ship-part:weapon:mass-driver", "Basic projectile firing weapon.");
        

            var entities = provider.GetRequiredService<EntityLoader>();

            #region Register ShipParts
            #region Register Hulls

            #region Triangle
            var triangle = new ShipPartConfiguration();
            triangle.AddPolygon(3);
            triangle.Flush();

            entities.TryRegister<Hull>(
                handle: "entity:ship-part:hull:triangle",
                setup: h =>
                {
                    h.Configuration = triangle;
                });
            #endregion

            #region Square
            var square = new ShipPartConfiguration();
            square.AddPolygon(4);
            square.Flush();

            entities.TryRegister<Hull>(
                handle: "entity:ship-part:hull:square",
                setup: h =>
                {
                    h.Configuration = square;
                });
            #endregion

            #region Hexagon
            var hexagon = new ShipPartConfiguration();
            hexagon.AddPolygon(6);
            hexagon.Flush();

            entities.TryRegister<Hull>(
                handle: "entity:ship-part:hull:hexagon",
                setup: h =>
                {
                    h.Configuration = hexagon;
                });
            #endregion

            #region Pentagon
            var pentagon = new ShipPartConfiguration();
            pentagon.AddSide(MathHelper.ToRadians(0), ShipPartConfiguration.NodeType.Male);
            pentagon.AddSide(MathHelper.ToRadians(90), ShipPartConfiguration.NodeType.Female);
            pentagon.AddSide(MathHelper.ToRadians(150), ShipPartConfiguration.NodeType.Female);
            pentagon.AddSide(MathHelper.ToRadians(60), ShipPartConfiguration.NodeType.Female);
            pentagon.AddSide(MathHelper.ToRadians(150), ShipPartConfiguration.NodeType.Female);
            pentagon.Flush();

            entities.TryRegister<Hull>(
                handle: "entity:ship-part:hull:pentagon",
                setup: h =>
                {
                    h.Configuration = pentagon;
                });
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
                setup: c =>
                {
                    c.Configuration = mosquito;
                });
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
                setup: t =>
                {
                    t.Configuration = thruster;
                });
            #endregion

            #region Weapons
            var massDriver = new ShipPartConfiguration();
            massDriver.AddVertice(0f, -0.15f);
            massDriver.AddVertice(0f, 0.15f);
            massDriver.AddVertice(0.6f, 0.075f);
            massDriver.AddVertice(0.6f, -0.075f);
            massDriver.AddNode(0.15f, 0, MathHelper.Pi, ShipPartConfiguration.NodeType.Male);
            massDriver.Flush();

            entities.TryRegister<Gun>(
                handle: "entity:ship-part:weapon:mass-driver",
                setup: g =>
                {
                    g.Configuration = massDriver;
                    g.SwivelRange = MathHelper.PiOver2;
                    g.FireInterval = 250;
                    g.FireStrength = 10f;
                    g.FireEnergyCost = 1f;
                    g.ProjectileHandle = "entity:ammunition:projectile:mass-driver";
                });
            #endregion
            #endregion

            #region Register Ammunitions
            entities.TryRegister<Projectile>(
                handle: "entity:ammunition:projectile:mass-driver",
                setup: p =>
                {
                    p.MaxAge = 5000;
                });
            #endregion
        }
    }
}
