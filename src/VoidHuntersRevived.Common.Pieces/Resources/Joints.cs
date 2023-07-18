using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces.Utilities;

namespace VoidHuntersRevived.Common.Pieces.Resources
{
    public class Joints
    {
        public required FixLocation[] Locations { get; init; }

        public static Joints Polygon(int sides)
        {
            PolygonHelper.VertexAngle[] vertexAngles = PolygonHelper.CalculateVertexAngles(sides).ToArray();

            Joints joints = new Joints()
            {
                Locations = new FixLocation[sides]
            };

            for (int i=0; i<vertexAngles.Length; i++)
            {
                FixVector2 start = vertexAngles[i].FixedVertex;
                FixVector2 end = vertexAngles[(i + 1) % vertexAngles.Length].FixedVertex;
                FixVector2 center = (start + end) / (Fix64)2;

                joints.Locations[i] = new FixLocation(center, vertexAngles[i].Angle);
            }

            return joints;
        }
    }
}
