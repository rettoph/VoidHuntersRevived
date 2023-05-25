using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Common;

namespace Microsoft.Xna.Framework
{
    public static class Vector2Extensions
    {
        public static Matrix ToTranslation(this Vector2 value, float z = 0)
        {
            return Matrix.CreateTranslation(value.X, value.Y, z);
        }
    }
}
