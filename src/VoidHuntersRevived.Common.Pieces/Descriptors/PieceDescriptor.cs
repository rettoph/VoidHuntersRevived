using Microsoft.Xna.Framework;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Serialization.Components;

namespace VoidHuntersRevived.Common.Pieces.Descriptors
{
    public abstract class PieceDescriptor : VoidHuntersEntityDescriptor
    {
        public PieceDescriptor()
        {
            this.ExtendWith(new ComponentManager[]
            {
                new ComponentManager<Plug>(
                    builder: new ComponentBuilder<Plug>(in Plug.Default),
                    serializer: DefaultComponentSerializer<Plug>.Default),
                new ComponentManager<Coupling, CouplingSerializer>(
                    builder: new ComponentBuilder<Coupling>()),
                new ComponentManager<Node, NodeSerializer>(
                    builder: new ComponentBuilder<Node>()),
                new ComponentManager<Rigid>(
                    builder: new ComponentBuilder<Rigid>(),
                    serializer: new DefaultComponentSerializer<Rigid>(
                        writer: (writer, instance) =>
                        {
                            writer.WriteStruct(instance.Centeroid);
                            writer.WriteNativeDynamicArray(instance.Shapes, PolygonWriter);
                        },
                        reader: (reader, id) => new Rigid()
                        {
                            Centeroid = reader.ReadStruct<FixVector2>(),
                            Shapes = reader.ReadNativeDynamicArray<Polygon>(PolygonReader)
                        })),
                new ComponentManager<Visible>(
                    builder: new ComponentBuilder<Visible>(),
                    serializer: new DefaultComponentSerializer<Visible>(
                        writer: (writer, instance) =>
                        {
                            writer.WriteStruct(instance.Color);
                            writer.WriteNativeDynamicArray(instance.Shapes, ShapeWriter);
                            writer.WriteNativeDynamicArray(instance.Paths, ShapeWriter);
                        },
                        reader: (reader, id) => new Visible()
                        {
                            Color = reader.ReadStruct<EntityResource<Color>>(),
                            Shapes = reader.ReadNativeDynamicArray<Shape>(ShapeReader),
                            Paths = reader.ReadNativeDynamicArray<Shape>(ShapeReader)
                        })),
            });
        }

        private Shape ShapeReader(EntityReader reader)
        {
            return new Shape()
            {
                Vertices = reader.ReadNativeDynamicArray<Vector3>()
            };
        }

        private void ShapeWriter(EntityWriter writer, Shape shape)
        {
            writer.WriteNativeDynamicArray(shape.Vertices);
        }

        private Polygon PolygonReader(EntityReader reader)
        {
            return new Polygon()
            {
                Density = reader.ReadStruct<Fix64>(),
                Vertices = reader.ReadNativeDynamicArray<FixVector2>()
            };
        }

        private void PolygonWriter(EntityWriter writer, Polygon polygon)
        {
            writer.WriteStruct(polygon.Density);
            writer.WriteNativeDynamicArray(polygon.Vertices);
        }
    }
}
