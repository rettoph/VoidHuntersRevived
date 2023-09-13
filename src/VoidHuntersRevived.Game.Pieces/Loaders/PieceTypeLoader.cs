using Guppy.Attributes;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Loaders;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Game.Common;
using Colors = VoidHuntersRevived.Common.Resources.Colors;

namespace VoidHuntersRevived.Game.Pieces.Loaders
{
    [AutoLoad]
    public sealed class PieceTypeLoader : IEntityTypeLoader
    {
        public void Configure(IEntityTypeService entityTypes)
        {
            entityTypes.Configure(EntityTypes.Pieces.HullTriangle, configuration =>
            {
                configuration.InitializeComponent<Rigid>(Rigid.Polygon(Fix64.One, 3));
                configuration.InitializeComponent<Visible>(Visible.Polygon(3));
                configuration.InitializeComponent<Sockets<Location>>(Sockets<Location>.Polygon(3));
            });

            entityTypes.Configure(EntityTypes.Pieces.HullSquare, configuration =>
            {
                configuration.InitializeComponent<Rigid>(Rigid.Polygon(Fix64.One, 4));
                configuration.InitializeComponent<Visible>(Visible.Polygon(4));
                configuration.InitializeComponent<Sockets<Location>>(Sockets<Location>.Polygon(4));
            });

            entityTypes.Configure(EntityTypes.Pieces.Thruster, configuration =>
            {
                configuration.InitializeComponent<Plug>(new Plug()
                {
                    Location = new Location(FixVector2.Zero, Fix64.Pi)
                });
                configuration.InitializeComponent<Rigid>(new Rigid()
                {
                    Centeroid = FixVector2.Zero,
                    Shapes = new[]
                    {
                            new Polygon()
                            {
                                Vertices = new[]
                                {
                                    new FixVector2(0.25m, -0.2m),
                                    new FixVector2(0.25m, 0.2m),
                                    new FixVector2(-0.25m,0.35m),
                                    new FixVector2(-0.25m,-0.35m)
                                }.ToNativeDynamicArray()
                            }
                        }.ToNativeDynamicArray()
                });
                configuration.InitializeComponent<Visible>(new Visible()
                {
                    Shapes = new[] {
                            new Shape()
                            {
                                Vertices = new[]
                                {
                                    new Vector3(0.25f, -0.2f, 0),
                                    new Vector3(0.25f, 0.2f, 0),
                                    new Vector3(-0.25f,0.35f, 0),
                                    new Vector3(-0.25f,-0.35f, 0)
                                }.ToNativeDynamicArray()
                            }
                        }.ToNativeDynamicArray(),
                    Paths = new[] {
                            new Shape()
                            {
                                Vertices = new[]
                                {
                                    new Vector3(0.25f, -0.2f, 0),
                                    new Vector3(0.25f, 0.2f, 0),
                                    new Vector3(-0.25f,0.35f, 0),
                                    new Vector3(-0.25f,-0.35f, 0),
                                    new Vector3(0.25f, -0.2f, 0)
                                }.ToNativeDynamicArray()
                            }
                        }.ToNativeDynamicArray()
                });
            });
        }
    }
}
