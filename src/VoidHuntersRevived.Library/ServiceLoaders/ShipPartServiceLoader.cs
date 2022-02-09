using Guppy.Attributes;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.EntityComponent.DependencyInjection.Builders;
using Guppy.Extensions.System;
using Guppy.Interfaces;
using Guppy.Network.Builders;
using Guppy.ServiceLoaders;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tainicom.Aether.Physics2D;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Library.Contexts.ShipParts;
using VoidHuntersRevived.Library.Contexts.Utilities;
using VoidHuntersRevived.Library.Entities.ShipParts.Hulls;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;
using VoidHuntersRevived.Library.Globals.Constants;
using VoidHuntersRevived.Library.Messages.Network;
using VoidHuntersRevived.Library.Messages.Network.Packets;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class ShipPartServiceLoader : IServiceLoader, INetworkLoader
    {
        public void ConfigureNetwork(NetworkProviderBuilder network)
        {
            network.RegisterDataType<ShipPartPacket>()
                .SetReader(ShipPartPacket.Read)
                .SetWriter(ShipPartPacket.Write);
        }

        public void RegisterServices(AssemblyHelper assemblyHelper, ServiceProviderBuilder services)
        {
            // Do not gift wrap polygons with the engine
            Settings.UseConvexHullPolygons = false;

            services.RegisterService<ConnectionNode>()
                .SetLifetime(ServiceLifetime.Transient)
                .RegisterTypeFactory(factory => factory.SetDefaultConstructor<ConnectionNode>());

            services.RegisterService<Hull>()
                .SetLifetime(ServiceLifetime.Transient)
                .RegisterTypeFactory(factory => factory.SetDefaultConstructor<Hull>());

            services.RegisterService<Thruster>()
                .SetLifetime(ServiceLifetime.Transient)
                .RegisterTypeFactory(factory => factory.SetDefaultConstructor<Thruster>());

            services.RegisterSetup<ShipPartService>()
                .SetMethod((shipParts, p, c) =>
            {
                shipParts.RegisterContext(new ThrusterContext()
                {
                    Name = ShipParts.Thruster,
                    Color = Colors.ShipPartThrusterColor,
                    Centeroid = new Vector2(-0.3f, 0f),
                    Thrust = Vector2.UnitX * 20,
                    Shapes = new[]
                    {
                        new ShapeContext()
                        {
                            Data = new PolygonShape(
                                vertices: new Vertices(new Vector2[] {
                                    new Vector2(0f, -0.15f),
                                    new Vector2(0f, 0.15f),
                                    new Vector2(-0.3f, 0.25f),
                                    new Vector2(-0.3f, -0.25f)
                                }),
                                density: 1f)
                        }
                        
                    },
                    Paths = new[]
                    {
                        new Vector2[] {
                            new Vector2(0f, -0.15f),
                            new Vector2(0f, 0.15f),
                            new Vector2(-0.3f, 0.25f),
                            new Vector2(-0.3f, -0.25f),
                            new Vector2(0f, -0.15f)
                        }
                    },
                    ConnectionNodes = new ConnectionNodeContext[]
                    {
                        new ConnectionNodeContext()
                        {
                            Position = new Vector2(-0.1f, 0f),
                            Rotation = MathHelper.Pi
                        }
                    }
                });

                shipParts.RegisterContext(new HullContext()
                {
                    Name = ShipParts.HullSquare,
                    Color = Colors.ShipPartHullColor,
                    Centeroid = PolygonHelper.GetCenteroid(4),
                    Shapes = new[]
                    {
                        new ShapeContext()
                        {
                            Data = new PolygonShape(
                                vertices: PolygonHelper.GetVertices(4),
                                density: 1f)
                        }
                    },
                    Paths = new[]
                    {
                        PolygonHelper.GetPath(4)
                    },
                    ConnectionNodes = PolygonHelper.GetConnectionNodes(4)
                });

                shipParts.RegisterContext(new HullContext()
                {
                    Name = ShipParts.HullTriangle,
                    Color = Colors.ShipPartHullColor,
                    Centeroid = PolygonHelper.GetCenteroid(3),
                    Shapes = new[]
                    {
                        new ShapeContext()
                        {
                            Data = new PolygonShape(
                                vertices: PolygonHelper.GetVertices(3),
                                density: 1f)
                        }
                    },
                    Paths = new[]
                    {
                        PolygonHelper.GetPath(3)
                    },
                    ConnectionNodes = PolygonHelper.GetConnectionNodes(3)
                });

                shipParts.ExportAll("export");
            });
        }
    }
}
