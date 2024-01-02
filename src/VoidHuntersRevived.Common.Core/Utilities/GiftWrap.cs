using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Common.Core.Utilities
{
    public static class GiftWrap
    {
        /// <summary>
        /// Returns the convex hull from the given vertices.
        /// </summary>
        /// <param name="vertices">The vertices.</param>
        public static Vector2[] GetConvexHull(Vector2[] vertices)
        {
            if (vertices.Length <= 3)
                return vertices;

            // Find the right most point on the hull
            int i0 = 0;
            float x0 = vertices[0].X;
            for (int i = 1; i < vertices.Length; ++i)
            {
                float x = vertices[i].X;
                if (x > x0 || (x == x0 && vertices[i].Y < vertices[i0].Y))
                {
                    i0 = i;
                    x0 = x;
                }
            }

            int[] hull = new int[vertices.Length];
            int m = 0;
            int ih = i0;

            for (; ; )
            {
                hull[m] = ih;

                int ie = 0;
                for (int j = 1; j < vertices.Length; ++j)
                {
                    if (ie == ih)
                    {
                        ie = j;
                        continue;
                    }

                    Vector2 r = vertices[ie] - vertices[hull[m]];
                    Vector2 v = vertices[j] - vertices[hull[m]];
                    float c = Cross(ref r, ref v);
                    if (c < 0)
                    {
                        ie = j;
                    }

                    // Collinearity check
                    if (c == 0 && v.LengthSquared() > r.LengthSquared())
                    {
                        ie = j;
                    }
                }

                ++m;
                ih = ie;

                if (ie == i0)
                {
                    break;
                }
            }

            Vector2[] result = new Vector2[m];

            // Copy vertices.
            for (int i = 0; i < m; ++i)
            {
                result[i] = vertices[hull[i]];
            }
            return result;
        }


        private static float Cross(ref Vector2 a, ref Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }

    }
}
