using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Guppy;
using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Configurations;
using VoidHuntersRevived.Library.Entities.Ammunitions;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;
using VoidHuntersRevived.Library.Entities.ShipParts.Weapons;

namespace VoidHuntersRevived.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class ShipPartServiceLoader : IServiceLoader
    {
        public void RegisterServices(ServiceCollection services)
        {
            Settings.MaxPolygonVertices = 16;

            services.AddFactory<RigidShipPart>(p => new RigidShipPart());

            #region Hulls
            #region Triangle
            var triangle = new ShipPartConfiguration();
            triangle.AddPolygon(3);
            triangle.Flush();

            services.AddTransient<RigidShipPart>("entity:ship-part:hull:triangle");
            services.AddSetup<RigidShipPart>("entity:ship-part:hull:triangle", (s, p, c) =>
            {
                s.Configuration = triangle;
            });
            #endregion

            #region Square
            var square = new ShipPartConfiguration();
            square.AddPolygon(4);
            square.Flush();


            services.AddTransient<RigidShipPart>("entity:ship-part:hull:square");
            services.AddSetup<RigidShipPart>("entity:ship-part:hull:square", (s, p, c) =>
            {
                s.Configuration = square;
            });
            #endregion

            #region Hexagon
            var hexagon = new ShipPartConfiguration();
            hexagon.AddPolygon(6);
            hexagon.Flush();

            services.AddTransient<RigidShipPart>("entity:ship-part:hull:hexagon");
            services.AddSetup<RigidShipPart>("entity:ship-part:hull:hexagon", (s, p, c) =>
            {
                s.Configuration = hexagon;
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

            services.AddTransient<RigidShipPart>("entity:ship-part:hull:pentagon");
            services.AddSetup<RigidShipPart>("entity:ship-part:hull:pentagon", (s, p, c) =>
            {
                s.Configuration = pentagon;
            });
            #endregion

            #region Vertical Beam
            var vBeam = new ShipPartConfiguration();
            vBeam.AddSide(MathHelper.ToRadians(0), ShipPartConfiguration.NodeType.Male);
            vBeam.AddSide(MathHelper.ToRadians(90), ShipPartConfiguration.NodeType.Female);
            vBeam.AddSide(MathHelper.ToRadians(180), ShipPartConfiguration.NodeType.Female);
            vBeam.AddSide(MathHelper.ToRadians(180), ShipPartConfiguration.NodeType.Female);
            vBeam.AddSide(MathHelper.ToRadians(90), ShipPartConfiguration.NodeType.Female);
            vBeam.AddSide(MathHelper.ToRadians(90), ShipPartConfiguration.NodeType.Female);
            vBeam.AddSide(MathHelper.ToRadians(180), ShipPartConfiguration.NodeType.Female);
            vBeam.AddSide(MathHelper.ToRadians(180), ShipPartConfiguration.NodeType.Female);
            vBeam.Flush();

            services.AddTransient<RigidShipPart>("entity:ship-part:hull:beam:vertical");
            services.AddSetup<RigidShipPart>("entity:ship-part:hull:beam:vertical", (s, p, c) =>
            {
                s.Configuration = vBeam;
            });
            #endregion

            #region Vertical Beam
            var hBeam = new ShipPartConfiguration();
            hBeam.AddSide(MathHelper.ToRadians(180), ShipPartConfiguration.NodeType.Male);
            hBeam.AddSide(MathHelper.ToRadians(180), ShipPartConfiguration.NodeType.Female);
            hBeam.AddSide(MathHelper.ToRadians(90), ShipPartConfiguration.NodeType.Female);
            hBeam.AddSide(MathHelper.ToRadians(90), ShipPartConfiguration.NodeType.Female);
            hBeam.AddSide(MathHelper.ToRadians(180), ShipPartConfiguration.NodeType.Female);
            hBeam.AddSide(MathHelper.ToRadians(180), ShipPartConfiguration.NodeType.Female);
            hBeam.AddSide(MathHelper.ToRadians(90), ShipPartConfiguration.NodeType.Female);
            hBeam.AddSide(MathHelper.ToRadians(90), ShipPartConfiguration.NodeType.Female);
            hBeam.Flush();

            services.AddTransient<RigidShipPart>("entity:ship-part:hull:beam:horizontal");
            services.AddSetup<RigidShipPart>("entity:ship-part:hull:beam:horizontal", (s, p, c) =>
            {
                s.Configuration = hBeam;
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
            mosquito.SetHull(new Vector2[]
            {
                mosquito.Vertices.ElementAt(0)[1],
                mosquito.Vertices.ElementAt(0)[2],
                mosquito.Vertices.ElementAt(0)[3],
                mosquito.Vertices.ElementAt(0)[4],
                mosquito.Vertices.ElementAt(0)[5],
                mosquito.Vertices.ElementAt(0)[6],
                mosquito.Vertices.ElementAt(1)[0],
                mosquito.Vertices.ElementAt(1)[1],
                mosquito.Vertices.ElementAt(1)[2],
                mosquito.Vertices.ElementAt(1)[3],
                mosquito.Vertices.ElementAt(1)[4],
            });

            services.AddTransient<RigidShipPart>("entity:ship-part:chassis:mosquito");
            services.AddSetup<RigidShipPart>("entity:ship-part:chassis:mosquito", (s, p, c) =>
            {
                s.Configuration = mosquito;
            });
            #endregion
            #endregion

            #region Thrusters
            services.AddFactory<Thruster>(p => new Thruster());

            #region Thruster
            var thruster = new ShipPartConfiguration();
            thruster.DefaultColor = Color.LimeGreen;
            thruster.AddVertex(-0.1f, 0.3f);
            thruster.AddVertex(-0.1f, -0.3f);
            thruster.AddVertex(0.4f, -0.1f);
            thruster.AddVertex(0.4f, 0.1f);
            thruster.AddNode(0.3f, 0, 0, ShipPartConfiguration.NodeType.Male);
            thruster.Flush();

            services.AddTransient<Thruster>("entity:ship-part:thruster:small");
            services.AddSetup<Thruster>("entity:ship-part:thruster:small", (s, p, c) =>
            {
                s.Configuration = thruster;
            });
            #endregion
            #endregion

            #region Weapons
            services.AddFactory<Gun>(p => new Gun());

            #region Weapon
            var massDriver = new ShipPartConfiguration();
            massDriver.DefaultColor = Color.Red;
            massDriver.AddVertex(-0.75f, 0.1f);
            massDriver.AddVertex(-0.75f, -0.1f);
            massDriver.AddVertex(0.1f, -0.2f);
            massDriver.AddVertex(0.1f, 0.2f);
            massDriver.AddNode(0, 0, 0, ShipPartConfiguration.NodeType.Male);
            massDriver.Flush();

            services.AddTransient<Gun>("entity:ship-part:weapon:mass-driver");
            services.AddSetup<Gun>("entity:ship-part:weapon:mass-driver", (s, p, c) =>
            {
                s.Configuration = massDriver;
            });
            #endregion
            #endregion

            #region Ammunitions
            #region Bullet
            services.AddFactory<Bullet>(b => new Bullet());
            services.AddTransient<Bullet>();
            #endregion
            #endregion
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
