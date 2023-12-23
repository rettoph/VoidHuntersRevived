using Guppy.Attributes;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Pieces.Components;

namespace VoidHuntersRevived.Common.Pieces.Serialization.Components
{
    [AutoLoad]
    public sealed class RigidComponentSerializer : DoNotSerializeComponentSerializer<Rigid>
    {
        //protected override Rigid Read(in DeserializationOptions options, EntityReader reader, in EntityId id)
        //{
        //    return new Rigid()
        //    {
        //        Centeroid = reader.ReadStruct<FixVector2>(),
        //        Shapes = reader.ReadNativeDynamicArray<Polygon>(PolygonReader, in options)
        //    };
        //}
        //
        //protected override void Write(EntityWriter writer, in Rigid instance, in SerializationOptions options)
        //{
        //    writer.WriteStruct(instance.Centeroid);
        //    writer.WriteNativeDynamicArray(instance.Shapes, PolygonWriter, options);
        //}
        //
        //private Polygon PolygonReader(DeserializationOptions options, EntityReader reader)
        //{
        //    return new Polygon()
        //    {
        //        Density = reader.ReadStruct<Fix64>(),
        //        Vertices = reader.ReadNativeDynamicArray<FixVector2>()
        //    };
        //}
        //
        //private void PolygonWriter(EntityWriter writer, Polygon polygon, SerializationOptions options)
        //{
        //    writer.WriteStruct(polygon.Density);
        //    writer.WriteNativeDynamicArray(polygon.Vertices);
        //}
    }
}
