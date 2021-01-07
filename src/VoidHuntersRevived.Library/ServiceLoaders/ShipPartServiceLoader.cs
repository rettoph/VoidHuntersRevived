using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Linq;
using tainicom.Aether.Physics2D;
using VoidHuntersRevived.Library.Contexts;
using VoidHuntersRevived.Library.Entities.Ammunitions;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.Armors;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;
using VoidHuntersRevived.Library.Entities.ShipParts.Weapons;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class ShipPartServiceLoader : IServiceLoader
    {
        public void RegisterServices(ServiceCollection services)
        {
            Settings.MaxPolygonVertices = 9;
            Settings.ContinuousPhysics = false;
            Directory.CreateDirectory("Resources/ShipParts");
            String DefaultShipPartLocation = "Resources/ShipParts";
            Boolean RefreshShipParts = false;
#if DEBUG
            RefreshShipParts = true;
#endif

            services.AddFactory<ShipPartService>(p => new ShipPartService());
            services.AddScoped<ShipPartService>();
            services.AddSetup<ShipPartService>((shipParts, p, c) =>
            {
                shipParts.ImportAll(DefaultShipPartLocation);
            });

            services.AddFactory<RigidShipPart>(p => new RigidShipPart());
            services.AddFactory<Thruster>(p => new Thruster());
            services.AddFactory<Gun>(p => new Gun());
            services.AddFactory<Bullet>(b => new Bullet());
            services.AddFactory<Armor>(b => new Armor());
            services.AddTransient<RigidShipPart>("entity:ship-part:rigid-ship-part");
            services.AddTransient<Thruster>("entity:ship-part:thruster");
            services.AddTransient<Gun>("entity:ship-part:weapon:gun");
            services.AddTransient<Bullet>();
            services.AddTransient<Armor>("entity:ship-part:armor");

            #region Default ShipPart File Generation
            #region Hulls
            #region Triangle
            if (!File.Exists($"{DefaultShipPartLocation}/hull.triangle.vhsp") || RefreshShipParts)
            { // Generate the default part...
                var triangle = new RigidShipPartContext("hull:triangle");
                triangle.Shapes.AddPolygon(3);
                triangle.Export($"{DefaultShipPartLocation}/hull.triangle.vhsp");
            }
            #endregion

            #region Square
            if (!File.Exists($"{DefaultShipPartLocation}/hull.square.vhsp") || RefreshShipParts)
            { // Generate the default part...
                var square = new RigidShipPartContext("hull:square");
                square.Shapes.AddPolygon(4);
                square.Export($"{DefaultShipPartLocation}/hull.square.vhsp");
            }
            #endregion

            #region Hexagon
            if (!File.Exists($"{DefaultShipPartLocation}/hull.hexagon.vhsp") || RefreshShipParts)
            { // Generate the default part...
                var hexagon = new RigidShipPartContext("hull:hexagon");
                hexagon.Shapes.AddPolygon(6);
                hexagon.Export($"{DefaultShipPartLocation}/hull.hexagon.vhsp");
            }
            #endregion

            #region Pentagon
            if (!File.Exists($"{DefaultShipPartLocation}/hull.pentagon.vhsp") || RefreshShipParts)
            { // Generate the default part...
                var pentagon = new RigidShipPartContext("hull:pentagon");
                pentagon.Shapes.TryAdd(builder =>
                {
                    builder.AddSide(MathHelper.ToRadians(0));
                    builder.AddSide(MathHelper.ToRadians(90));
                    builder.AddSide(MathHelper.ToRadians(150));
                    builder.AddSide(MathHelper.ToRadians(60));
                    builder.AddSide(MathHelper.ToRadians(150));
                });
                pentagon.Export($"{DefaultShipPartLocation}/hull.pentagon.vhsp");
            }
            #endregion

            #region Diamond
            if (!File.Exists($"{DefaultShipPartLocation}/hull.diamond.vhsp") || RefreshShipParts)
            { // Generate the default part...
                var diamond = new RigidShipPartContext("hull:diamond");
                diamond.Shapes.TryAdd(builder =>
                {
                    builder.AddSide(MathHelper.ToRadians(0));
                    builder.AddSide(MathHelper.ToRadians(150));
                    builder.AddSide(MathHelper.ToRadians(60));
                    builder.AddSide(MathHelper.ToRadians(150));
                    builder.AddSide(MathHelper.ToRadians(150));
                    builder.AddSide(MathHelper.ToRadians(60));
                    builder.AddSide(MathHelper.ToRadians(150));
                });
                diamond.Export($"{DefaultShipPartLocation}/hull.diamond.vhsp");
            }
            #endregion

            #region Vertical Beam
            if (!File.Exists($"{DefaultShipPartLocation}/hull.beam.vertical.vhsp") || RefreshShipParts)
            { // Generate the default part...
                var vBeam = new RigidShipPartContext("hull:beam:vertical");
                vBeam.Shapes.TryAdd(builder =>
                {
                    builder.AddSide(MathHelper.ToRadians(0));
                    builder.AddSide(MathHelper.ToRadians(90), 3);
                    builder.AddSide(MathHelper.ToRadians(90));
                    builder.AddSide(MathHelper.ToRadians(90), 3);
                });
                vBeam.Export($"{DefaultShipPartLocation}/hull.beam.vertical.vhsp");
            }
            #endregion

            #region Horizontal Beam
            if (!File.Exists($"{DefaultShipPartLocation}/hull.beam.horizontal.vhsp") || RefreshShipParts)
            { // Generate the default part...
                var hBeam = new RigidShipPartContext("hull:beam:horizontal");
                hBeam.Shapes.MaleConnectionNode = new ConnectionNodeContext()
                {
                    Position = new Vector2(-1.5f, 0),
                    Rotation = MathHelper.PiOver2
                };
                hBeam.Shapes.TryAdd(builder =>
                {
                    builder.AddSide(MathHelper.ToRadians(0), 3);
                    builder.AddSide(MathHelper.ToRadians(90));
                    builder.AddSide(MathHelper.ToRadians(90), 3);
                    builder.AddSide(MathHelper.ToRadians(90));
                });
                hBeam.Export($"{DefaultShipPartLocation}/hull.beam.horizontal.vhsp");
            }
            #endregion
            #endregion

            #region Chassis
            #region Mosquito
            if (!File.Exists($"{DefaultShipPartLocation}/chassis.mosquito.vhsp") || RefreshShipParts)
            { // Generate the default part...
                var mosquito = new RigidShipPartContext("chassis:mosquito");
                mosquito.Shapes.MaleConnectionNode = new ConnectionNodeContext()
                {
                    Position = new Vector2(0, 1f),
                    Rotation = 0
                };
                mosquito.Shapes.TryAdd(builder =>
                {
                    builder.AddSide(MathHelper.ToRadians(0), 2, false);
                    builder.AddSide(MathHelper.ToRadians(120));
                    builder.AddSide(MathHelper.ToRadians(120));
                    builder.AddSide(MathHelper.ToRadians(120), 2);
                    builder.AddSide(MathHelper.ToRadians(120));
                    builder.AddSide(MathHelper.ToRadians(120));
                    builder.Rotation = MathHelper.ToRadians(180 + 90);
                });

                mosquito.Shapes.TryAdd(builder =>
                {
                    builder.AddSide(MathHelper.ToRadians(0));
                    builder.AddSide(MathHelper.ToRadians(150));
                    builder.AddSide(MathHelper.ToRadians(120));
                    builder.AddSide(MathHelper.ToRadians(120));
                    builder.AddSide(MathHelper.ToRadians(150));
                    builder.Rotation = MathHelper.ToRadians(180);
                });

                mosquito.Shapes.SetHull(
                    mosquito.Shapes.ElementAt(0).Vertices[1],
                    mosquito.Shapes.ElementAt(0).Vertices[2],
                    mosquito.Shapes.ElementAt(0).Vertices[3],
                    mosquito.Shapes.ElementAt(0).Vertices[4],
                    mosquito.Shapes.ElementAt(0).Vertices[5],
                    mosquito.Shapes.ElementAt(0).Vertices[0],
                    mosquito.Shapes.ElementAt(1).Vertices[1],
                    mosquito.Shapes.ElementAt(1).Vertices[2],
                    mosquito.Shapes.ElementAt(1).Vertices[3],
                    mosquito.Shapes.ElementAt(1).Vertices[4]);
                mosquito.Export($"{DefaultShipPartLocation}/chassis.mosquito.vhsp");
            }
            #endregion

            #region Pelican
            if (!File.Exists($"{DefaultShipPartLocation}/chassis.pelican.vhsp") || RefreshShipParts)
            { // Generate the default part...
                var pelican = new RigidShipPartContext("chassis:pelican");
                pelican.Shapes.MaleConnectionNode = new ConnectionNodeContext()
                {
                    Position = new Vector2(1f, -0.5f),
                    Rotation = 0
                };
                pelican.Shapes.TryAdd(builder =>
                {
                    builder.AddSide(MathHelper.ToRadians(0), 1, false);
                    builder.AddSide(MathHelper.ToRadians(120));
                    builder.AddSide(MathHelper.ToRadians(120));
                    builder.AddSide(MathHelper.ToRadians(120));
                    builder.AddSide(MathHelper.ToRadians(120));
                    builder.AddSide(MathHelper.ToRadians(120));

                    builder.Rotation = -MathHelper.PiOver2;
                    builder.Translation = new Vector2(0, -1);
                });

                pelican.Shapes.TryAdd(builder =>
                {
                    builder.AddSide(MathHelper.ToRadians(0), 1, false);
                    builder.AddSide(MathHelper.ToRadians(90));
                    builder.AddSide(MathHelper.ToRadians(90), 1, false);
                    builder.AddSide(MathHelper.ToRadians(90));

                    builder.Rotation = MathHelper.PiOver2;
                });
                pelican.Shapes.TryAdd(builder =>
                {
                    builder.AddSide(MathHelper.ToRadians(0), 1, false);
                    builder.AddSide(MathHelper.ToRadians(120));
                    builder.AddSide(MathHelper.ToRadians(150), 2);
                    builder.AddSide(MathHelper.ToRadians(150));
                    builder.AddSide(MathHelper.ToRadians(120));
                    builder.AddSide(MathHelper.ToRadians(120));
                    builder.AddSide(MathHelper.ToRadians(150), 2);
                    builder.AddSide(MathHelper.ToRadians(150));

                    builder.Rotation = MathHelper.PiOver2;
                    builder.Translation = new Vector2(1, 0);
                });

                pelican.Shapes.SetHull(
                    pelican.Shapes.ElementAt(0).Vertices[1],
                    pelican.Shapes.ElementAt(0).Vertices[2],
                    pelican.Shapes.ElementAt(0).Vertices[3],
                    pelican.Shapes.ElementAt(0).Vertices[4],
                    pelican.Shapes.ElementAt(0).Vertices[5],
                    pelican.Shapes.ElementAt(1).Vertices[1],
                    pelican.Shapes.ElementAt(2).Vertices[1],
                    pelican.Shapes.ElementAt(2).Vertices[2],
                    pelican.Shapes.ElementAt(2).Vertices[3],
                    pelican.Shapes.ElementAt(2).Vertices[4],
                    pelican.Shapes.ElementAt(2).Vertices[5],
                    pelican.Shapes.ElementAt(2).Vertices[6],
                    pelican.Shapes.ElementAt(2).Vertices[7],
                    pelican.Shapes.ElementAt(1).Vertices[3]);

                pelican.Export($"{DefaultShipPartLocation}/chassis.pelican.vhsp");
            }
            #endregion
            #endregion

            #region Thrusters
            if (!File.Exists($"{DefaultShipPartLocation}/thruster.small.vhsp") || RefreshShipParts)
            { // Generate the default part...
                var smallThruster = new ThrusterContext("thruster:small");
                smallThruster.MaxImpulse = Vector2.UnitX * 10f;
                smallThruster.Shapes.MaleConnectionNode = new ConnectionNodeContext()
                {
                    Position = new Vector2(0, 0),
                    Rotation = 0
                };
                smallThruster.Shapes.TryAdd(builder =>
                {
                    builder.AddSide(MathHelper.ToRadians(0), 3, false);
                    builder.AddSide(MathHelper.ToRadians(68.199f), 2.693f, false);
                    builder.AddSide(MathHelper.ToRadians(111.801f), 1, false);
                    builder.AddSide(MathHelper.ToRadians(111.801f), 2.693f, false);

                    builder.Translation = new Vector2(-1.75f, 1.5f);
                    builder.Scale = 0.2f;
                    builder.Rotation = MathHelper.PiOver2;
                });
                smallThruster.Export($"{DefaultShipPartLocation}/thruster.small.vhsp");
            }
            #endregion

            #region Weapons
            #region Mass Driver
            if (!File.Exists($"{DefaultShipPartLocation}/weapon.gun.mass-driver.vhsp") || RefreshShipParts)
            { // Generate the default part...
                var massDriver = new GunContext("weapon:gun:mass-driver");
                massDriver.SwivelRange = 2f;
                massDriver.BulletDamage = 10f;
                massDriver.Shapes.MaleConnectionNode = new ConnectionNodeContext()
                {
                    Position = new Vector2(0, 0),
                    Rotation = 0
                };
                massDriver.Shapes.TryAdd(builder =>
                {
                    builder.AddSide(MathHelper.ToRadians(0), 2, false);
                    builder.AddSide(MathHelper.ToRadians(83.29f), 4.279f, false);
                    builder.AddSide(MathHelper.ToRadians(96.71f), 1, false);
                    builder.AddSide(MathHelper.ToRadians(96.71f), 4.2793f, false);

                    builder.Translation = new Vector2(0.25f, -1f);
                    builder.Scale = 0.2f;
                    builder.Rotation = -MathHelper.PiOver2;
                });
                massDriver.Export($"{DefaultShipPartLocation}/weapon.gun.mass-driver.vhsp");
            }
            #endregion
            #endregion

            #region Armors
            #region Plate
            if (!File.Exists($"{DefaultShipPartLocation}/armor.plate.vhsp") || RefreshShipParts)
            { // Generate the default part...
                var armorPlate = new ArmorContext("armor:plate");
                armorPlate.Shapes.MaleConnectionNode = new ConnectionNodeContext()
                {
                    Position = new Vector2(0, 1.5f),
                    Rotation = 0
                };
                armorPlate.Shapes.TryAdd(builder =>
                {
                    builder.AddSide(MathHelper.ToRadians(0), 3, false);
                    builder.AddSide(MathHelper.ToRadians(60f), 1, false);
                    builder.AddSide(MathHelper.ToRadians(120f), 2, false);
                    builder.AddSide(MathHelper.ToRadians(120f), 1, false);

                    builder.Rotation = -MathHelper.PiOver2;
                });
                armorPlate.Export($"{DefaultShipPartLocation}/armor.plate.vhsp");
            }

            if (!File.Exists($"{DefaultShipPartLocation}/armor.shield.vhsp") || RefreshShipParts)
            { // Generate the default part...
                var armorShield = new ArmorContext("armor:shield");
                armorShield.Shapes.MaleConnectionNode = new ConnectionNodeContext()
                {
                    Position = new Vector2(0, 0),
                    Rotation = 0
                };
                armorShield.Shapes.AddPolygon(4, false, builder =>
                {
                    builder.Translation = new Vector2(0, -0.5f);
                });
                armorShield.Shapes.TryAdd(builder =>
                {
                    builder.AddSide(MathHelper.ToRadians(180), 1, false);
                    builder.AddSide(MathHelper.ToRadians(150), 1, false);
                    builder.AddSide(MathHelper.ToRadians(90), 1, false);
                    builder.AddSide(MathHelper.ToRadians(90), 1, false);
                    builder.AddSide(MathHelper.ToRadians(150), 1, false);
                    // builder.AddSide(MathHelper.ToRadians(60));

                    builder.Translation = new Vector2(-1, 0.5f);
                });

                armorShield.Shapes.TryAdd(builder =>
                {
                    builder.AddSide(MathHelper.ToRadians(180), 1, false);
                    builder.AddSide(MathHelper.ToRadians(150), 1, false);
                    builder.AddSide(MathHelper.ToRadians(90), 1, false);
                    builder.AddSide(MathHelper.ToRadians(90), 1, false);
                    builder.AddSide(MathHelper.ToRadians(150), 1, false);
                    // builder.AddSide(MathHelper.ToRadians(60));

                    builder.Translation = new Vector2(-1, -0.5f);
                    builder.Rotation = MathHelper.Pi + MathHelper.PiOver2 + (MathHelper.Pi / 6);
                });

                armorShield.Shapes.SetHull(
                    armorShield.Shapes.ElementAt(2).Vertices[4],
                    armorShield.Shapes.ElementAt(2).Vertices[3],
                    armorShield.Shapes.ElementAt(2).Vertices[2],
                    armorShield.Shapes.ElementAt(2).Vertices[1],
                    armorShield.Shapes.ElementAt(2).Vertices[0],
                    armorShield.Shapes.ElementAt(1).Vertices[0],
                    armorShield.Shapes.ElementAt(1).Vertices[4],
                    armorShield.Shapes.ElementAt(1).Vertices[3],
                    armorShield.Shapes.ElementAt(1).Vertices[2],
                    armorShield.Shapes.ElementAt(1).Vertices[1]);
                armorShield.Export($"{DefaultShipPartLocation}/armor.shield.vhsp");
            }
            #endregion
            #endregion
            #endregion
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
