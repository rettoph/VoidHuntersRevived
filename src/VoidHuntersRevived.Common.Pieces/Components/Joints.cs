using Svelto.Common;
using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Pieces.Resources;
using VoidHuntersRevived.Common.Pieces.Utilities;

namespace VoidHuntersRevived.Common.Pieces.Components
{
    public struct Joints : IEntityComponent
    {
        public readonly NativeDynamicArrayCast<Joint> Items;

        public Joints(uint count) : this()
        {
            this.Items = new NativeDynamicArrayCast<Joint>(count, Allocator.Managed);
        }

        public static Joints Polygon(int sides)
        {
            Vertices vertices = new Vertices(PolygonHelper.CalculateVertexAngles(sides).Select(x => x.FixedVertex).ToArray());

            Joints joints = new Joints((uint)sides);

            return joints;
        }
    }
}
