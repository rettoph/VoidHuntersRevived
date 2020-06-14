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

        public static Vector2 Round(this Vector2 vector)
        {
            return new Vector2((Single)Math.Round(vector.X), (Single)Math.Round(vector.Y));
        }

        public static Vector2 Rotate(this Vector2 v, Single delta)
        {
            return v.RotateTo((Single)Math.Atan2(v.Y, v.X) + delta);
        }

        public static Vector2 RotateTo(this Vector2 v, Single target)
        {
            var l = v.Length();

            return new Vector2(
                x: (Single)Math.Cos(target) * l,
                y: (Single)Math.Sin(target) * l);
        }
    }
}
