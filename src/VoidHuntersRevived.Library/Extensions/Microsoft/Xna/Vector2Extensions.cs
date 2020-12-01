using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Extensions.Microsoft.Xna
{
    public static class Vector2Extensions
    {
        public static Vector3 ToVector3(this Vector2 vector, Single z = 0)
        {
            return new Vector3(vector.X, vector.Y, z);
        }
    }
}
