using Microsoft.Xna.Framework;
using Svelto.Common;
using Svelto.DataStructures;
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
using VoidHuntersRevived.Common.Pieces.Resources;

namespace VoidHuntersRevived.Common.Pieces.Descriptors
{
    public class HullDescriptor : PieceDescriptor
    {
        public HullDescriptor()
        {
            this.ExtendWith(new ComponentManager[]
            {
                new ComponentManager<Rigid>(
                    builder: new ComponentBuilder<Rigid>(),
                    serializer: new ComponentSerializer<Rigid>(
                        writer: (writer, instance) =>
                        {
                            writer.WriteNativeDynamicArray(instance.Shapes, PolygonWriter);
                        },
                        reader: (seed, reader) =>
                        {
                            Rigid rigid = new Rigid()
                            {
                                Shapes = reader.ReadNativeDynamicArray<Polygon>(PolygonReader)
                            };

                            return rigid;
                        })),
                new ComponentManager<Visible>(
                    builder: new ComponentBuilder<Visible>(),
                    serializer: new ComponentSerializer<Visible>(
                        writer: (writer, instance) =>
                        {
                            writer.WriteNativeDynamicArray(instance.Shapes, ShapeWriter);
                            writer.WriteNativeDynamicArray(instance.Paths, ShapeWriter);
                        },
                        reader: (seed, reader) =>
                        {
                            Visible visible = new Visible()
                            {
                                Shapes = reader.ReadNativeDynamicArray<Shape>(ShapeReader),
                                Paths = reader.ReadNativeDynamicArray<Shape>(ShapeReader)
                            };

                            return visible;
                        }))
            });
        }

        private Shape ShapeReader(EntityReader reader)
        {
            return new Shape()
            {
                Color = reader.ReadUnmanaged<EntityResource<Color>>(),
                Vertices = reader.ReadNativeDynamicArray<Vector3>()
            };
        }

        private void ShapeWriter(EntityWriter writer, Shape shape)
        {
            writer.WriteUnmanaged(shape.Color);
            writer.WriteNativeDynamicArray(shape.Vertices);
        }

        private Polygon PolygonReader(EntityReader reader)
        {
            return new Polygon()
            {
                Density = reader.ReadUnmanaged<Fix64>(),
                Vertices = reader.ReadNativeDynamicArray<FixVector2>()
            };
        }

        private void PolygonWriter(EntityWriter writer, Polygon polygon)
        {
            writer.WriteUnmanaged(polygon.Density);
            writer.WriteNativeDynamicArray(polygon.Vertices);
        }
    }
}
