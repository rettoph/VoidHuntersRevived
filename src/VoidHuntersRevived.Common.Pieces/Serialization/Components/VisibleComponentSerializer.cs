using Guppy.Attributes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Pieces.Components;

namespace VoidHuntersRevived.Common.Pieces.Serialization.Components
{
    [AutoLoad]
    public sealed class VisibleComponentSerializer : ComponentSerializer<Visible>
    {
        protected override Visible Read(EntityReader reader, EntityId id)
        {
            return new Visible()
            {
                Color = reader.ReadStruct<EntityResource<Color>>(),
                Shapes = reader.ReadNativeDynamicArray<Shape>(ShapeReader),
                Paths = reader.ReadNativeDynamicArray<Shape>(ShapeReader)
            };
        }

        protected override void Write(EntityWriter writer, Visible instance)
        {
            writer.WriteStruct(instance.Color);
            writer.WriteNativeDynamicArray(instance.Shapes, ShapeWriter);
            writer.WriteNativeDynamicArray(instance.Paths, ShapeWriter);
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
    }
}
