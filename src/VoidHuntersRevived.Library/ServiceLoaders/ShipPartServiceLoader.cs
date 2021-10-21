using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Extensions.System;
using Guppy.Interfaces;
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
using VoidHuntersRevived.Library.Dtos.Utilities;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.Hulls;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class ShipPartServiceLoader : IServiceLoader
    {
        public void RegisterServices(GuppyServiceCollection services)
        {
            // Do not gift wrap polygons with the engine
            Settings.UseConvexHullPolygons = false;

            #region Services
            services.RegisterTypeFactory<ConnectionNode>(p => new ConnectionNode());

            services.RegisterTransient<ConnectionNode>();
            #endregion

            #region Entities
            services.RegisterTypeFactory<Hull>(p => new Hull());
            services.RegisterTypeFactory<Thruster>(p => new Thruster());

            services.RegisterTransient(Constants.ServiceConfigurationKeys.ShipParts.Hull, typeof(Hull));
            services.RegisterTransient(Constants.ServiceConfigurationKeys.ShipParts.Thruster, typeof(Thruster));
            #endregion

            services.RegisterSetup<ShipPartService>((shipParts, p, c) =>
            {
                shipParts.RegisterContext(new ThrusterContext()
                {
                    Name = "ship-part:hull:thruster",
                    Color = Constants.Colors.ShipPartThrusterColor,
                    Centeroid = new Vector2(-0.3f, 0f),
                    Shapes = new[]
                    {
                        new ShapeDto()
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
                    ConnectionNodes = new ConnectionNodeDto[]
                    {
                        new ConnectionNodeDto()
                        {
                            Position = new Vector2(-0.1f, 0f),
                            Rotation = MathHelper.Pi
                        }
                    }
                });

                shipParts.RegisterContext(new HullContext()
                {
                    Name = "ship-part:hull:square",
                    Color = Constants.Colors.ShipPartHullColor,
                    Centeroid = PolygonHelper.GetCenteroid(4),
                    Shapes = new[]
                    {
                        new ShapeDto()
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
                    Name = "ship-part:hull:triangle",
                    Color = Constants.Colors.ShipPartHullColor,
                    Centeroid = PolygonHelper.GetCenteroid(3),
                    Shapes = new[]
                    {
                        new ShapeDto()
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

        public void ConfigureProvider(GuppyServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
