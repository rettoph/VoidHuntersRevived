using Guppy.Attributes;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Loaders;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Services;

namespace VoidHuntersRevived.Domain.Pieces.Loaders
{
    [AutoLoad]
    internal sealed class PieceTypeLoader : IEntityTypeLoader
    {
        private readonly IPieceTypeService _pieces;

        public PieceTypeLoader(IPieceTypeService pieces)
        {
            _pieces = pieces;
        }

        public void Configure(IEntityTypeService entityTypes)
        {
            //var rigid = Rigid.Polygon(Fix64.One, 3);
            //var visible = Sockets<Location>.Polygon(3);
            //var sockets = Sockets<Location>.Polygon(3);

            foreach (PieceType piece in _pieces.All())
            {
                entityTypes.Configure(piece.EntityType, configuration =>
                {
                    configuration.InitializeInstanceComponent(piece.Id);

                    foreach (IPieceComponent component in piece.InstanceComponents.Values)
                    {
                        configuration.InitializeInstanceComponent(component);
                    }

                    foreach (IPieceComponent component in piece.StaticComponents.Values)
                    {
                        configuration.InitializeStaticComponent(component);
                    }
                });
            }

            // entityTypes.Configure(EntityTypes.Pieces.HullTriangle, configuration =>
            // {
            //     configuration.InitializeComponent<Rigid>(Rigid.Polygon(Fix64.One, 3));
            //     configuration.InitializeComponent<Visible>(Visible.Polygon(3));
            //     configuration.InitializeComponent<Sockets<Location>>(Sockets<Location>.Polygon(3));
            // });

            // entityTypes.Configure(EntityTypes.Pieces.HullSquare, configuration =>
            // {
            //     configuration.InitializeComponent<Rigid>(Rigid.Polygon(Fix64.One, 4));
            //     configuration.InitializeComponent<Visible>(Visible.Polygon(4));
            //     configuration.InitializeComponent<Sockets<Location>>(Sockets<Location>.Polygon(4));
            // });

            //entityTypes.Configure(EntityTypes.Pieces.Thruster, configuration =>
            //{
            //    configuration.InitializeComponent<Plug>(new Plug()
            //    {
            //        Location = new Location(FixVector2.Zero, Fix64.Pi)
            //    });
            //    configuration.InitializeComponent<Rigid>(new Rigid()
            //    {
            //        Centeroid = FixVector2.Zero,
            //        Shapes = new[]
            //        {
            //                new Polygon()
            //                {
            //                    Vertices = new[]
            //                    {
            //                        new FixVector2(0.25m, -0.2m),
            //                        new FixVector2(0.25m, 0.2m),
            //                        new FixVector2(-0.25m,0.35m),
            //                        new FixVector2(-0.25m,-0.35m)
            //                    }.ToNativeDynamicArray()
            //                }
            //            }.ToNativeDynamicArray()
            //    });
            //    configuration.InitializeComponent<Visible>(new Visible()
            //    {
            //        Fill = new[] {
            //                new Shape()
            //                {
            //                    Vertices = new[]
            //                    {
            //                        new Vector3(0.25f, -0.2f, 0),
            //                        new Vector3(0.25f, 0.2f, 0),
            //                        new Vector3(-0.25f,0.35f, 0),
            //                        new Vector3(-0.25f,-0.35f, 0)
            //                    }.ToNativeDynamicArray()
            //                }
            //            }.ToNativeDynamicArray(),
            //        Trace = new[] {
            //                new Shape()
            //                {
            //                    Vertices = new[]
            //                    {
            //                        new Vector3(0.25f, -0.2f, 0),
            //                        new Vector3(0.25f, 0.2f, 0),
            //                        new Vector3(-0.25f,0.35f, 0),
            //                        new Vector3(-0.25f,-0.35f, 0),
            //                        new Vector3(0.25f, -0.2f, 0)
            //                    }.ToNativeDynamicArray()
            //                }
            //            }.ToNativeDynamicArray()
            //    });
            //});
        }
    }
}
