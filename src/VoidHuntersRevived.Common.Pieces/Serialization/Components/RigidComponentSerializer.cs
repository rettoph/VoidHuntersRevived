using Guppy.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Pieces.Components;

namespace VoidHuntersRevived.Common.Pieces.Serialization.Components
{
    [AutoLoad]
    public sealed class RigidComponentSerializer : ComponentSerializer<Rigid>
    {
        protected override Rigid Read(EntityReader reader, EntityId id)
        {
            return new Rigid()
            {
                Centeroid = reader.ReadStruct<FixVector2>(),
                Shapes = reader.ReadNativeDynamicArray<Polygon>(PolygonReader)
            };
        }

        protected override void Write(EntityWriter writer, Rigid instance)
        {
            writer.WriteStruct(instance.Centeroid);
            writer.WriteNativeDynamicArray(instance.Shapes, PolygonWriter);
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
