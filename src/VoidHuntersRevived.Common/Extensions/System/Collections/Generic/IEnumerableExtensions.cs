using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    public static class IEnumerableExtensions
    {
        public static Vector2 Average(this IEnumerable<Vector2> values)
        {
            int count = 0;
            Vector2 sum = Vector2.Zero;

            foreach(Vector2 vector in values)
            {
                sum += vector;
                count++;
            }

            return sum / count;
        }
    }
}
