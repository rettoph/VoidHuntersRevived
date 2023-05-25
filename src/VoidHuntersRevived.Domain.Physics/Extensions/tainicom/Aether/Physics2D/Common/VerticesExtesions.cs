using FixedMath.NET;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Physics.Extensions.tainicom.Aether.Physics2D.Common;

namespace tainicom.Aether.Physics2D.Common
{
    public static class VerticesExtesions
    {
        /// <summary>
        /// Transforms the polygon using the defined fixedmatrix.
        /// </summary>
        /// <param name="transform">The matrix to use as transformation.</param>
        public static void Transform(this Vertices vertices, ref FixMatrix transform)
        {
            // Transform main polygon
            for (int i = 0; i < vertices.Count; i++)
                vertices[i] = FixVector2.Transform(vertices[i].AsFixVector2(), transform).AsAetherVector2();

            // Transform holes
            if (vertices.Holes != null && vertices.Holes.Count > 0)
            {
                for (int i = 0; i < vertices.Holes.Count; i++)
                {
                    FixVector2[] temp = vertices.Holes[i].Select(x => x.AsFixVector2()).ToArray();
                    FixVector2.Transform(temp, ref transform, temp);

                    vertices.Holes[i] = new Vertices(temp.Select(x => x.AsAetherVector2()).ToArray());
                }
            }
        }
    }
}
