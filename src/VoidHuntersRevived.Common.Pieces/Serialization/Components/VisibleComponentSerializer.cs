﻿using Guppy.Attributes;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Options;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Pieces.Components;

namespace VoidHuntersRevived.Common.Pieces.Serialization.Components
{
    [AutoLoad]
    public sealed class VisibleComponentSerializer : DoNotSerializeComponentSerializer<Visible>
    {
        protected override Visible Read(in DeserializationOptions options, EntityReader reader, in EntityId id)
        {
            return new Visible()
            {
                Fill = reader.ReadNativeDynamicArray<Shape>(ShapeReader, options),
                Trace = reader.ReadNativeDynamicArray<Shape>(ShapeReader, options)
            };
        }

        protected override void Write(EntityWriter writer, in Visible instance, in SerializationOptions options)
        {
            writer.WriteNativeDynamicArray(instance.Fill, ShapeWriter, options);
            writer.WriteNativeDynamicArray(instance.Trace, ShapeWriter, options);
        }

        private Shape ShapeReader(DeserializationOptions options, EntityReader reader)
        {
            return new Shape()
            {
                Vertices = reader.ReadNativeDynamicArray<Vector2>()
            };
        }

        private void ShapeWriter(EntityWriter writer, Shape shape, SerializationOptions options)
        {
            writer.WriteNativeDynamicArray(shape.Vertices);
        }
    }
}
