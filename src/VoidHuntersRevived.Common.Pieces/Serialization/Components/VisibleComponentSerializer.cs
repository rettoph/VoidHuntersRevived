using Guppy.Attributes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Options;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Pieces.Components;

namespace VoidHuntersRevived.Common.Pieces.Serialization.Components
{
    [AutoLoad]
    public sealed class VisibleComponentSerializer : ComponentSerializer<Visible>
    {
        protected override Visible Read(in DeserializationOptions options, EntityReader reader, in EntityId id)
        {
            return new Visible()
            {
                Shapes = reader.ReadNativeDynamicArray<Shape>(ShapeReader, options),
                Paths = reader.ReadNativeDynamicArray<Shape>(ShapeReader, options)
            };
        }

        protected override void Write(EntityWriter writer, in Visible instance, in SerializationOptions options)
        {
            writer.WriteNativeDynamicArray(instance.Shapes, ShapeWriter, options);
            writer.WriteNativeDynamicArray(instance.Paths, ShapeWriter, options);
        }

        private Shape ShapeReader(DeserializationOptions options, EntityReader reader)
        {
            return new Shape()
            {
                Vertices = reader.ReadNativeDynamicArray<Vector3>()
            };
        }

        private void ShapeWriter(EntityWriter writer, Shape shape, SerializationOptions options)
        {
            writer.WriteNativeDynamicArray(shape.Vertices);
        }
    }
}
