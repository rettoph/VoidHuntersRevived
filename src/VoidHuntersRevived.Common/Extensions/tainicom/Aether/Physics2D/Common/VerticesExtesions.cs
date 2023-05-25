using FixedMath.NET;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Helpers;

namespace tainicom.Aether.Physics2D.Common
{
    public static class VerticesExtesions
    {
        /// <summary>
        /// Transforms the polygon using the defined fixedmatrix.
        /// </summary>
        /// <param name="transform">The matrix to use as transformation.</param>
        public static void Transform(this Vertices vertices, ref FixedMatrix transform)
        {
            // Transform main polygon
            for (int i = 0; i < vertices.Count; i++)
                vertices[i] = FixedVector2Helper.Transform(vertices[i], transform);

            // Transform holes
            if (vertices.Holes != null && vertices.Holes.Count > 0)
            {
                for (int i = 0; i < vertices.Holes.Count; i++)
                {
                    AetherVector2[] temp = vertices.Holes[i].ToArray();
                    FixedVector2Helper.Transform(temp, ref transform, temp);

                    vertices.Holes[i] = new Vertices(temp);
                }
            }
        }
    }
}
