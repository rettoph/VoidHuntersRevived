using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Xna.Framework
{
    public static class Vector2Extensions
    {
        public static Matrix GetTranslation(this Vector2 value)
        {
            return Matrix.CreateTranslation(value.X, value.Y, 0);
        }
    }
}
