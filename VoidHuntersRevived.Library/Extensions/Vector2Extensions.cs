using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Extensions
{
    public static class Vector2Extensions
    {
        public static Vector3 ToVector3(this Vector2 vector, float z)
        {
            return new Vector3(vector.X, vector.Y, z);
        }
    }
}
