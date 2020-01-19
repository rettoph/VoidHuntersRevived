using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Extensions.Farseer
{
    public static class Vector2Extensions
    {
        public static Vector2 Rotate(this Vector2 v, Single delta)
        {
            return v.Target((Single)Math.Atan2(v.Y, v.X) + delta);
        }

        public static Vector2 Target(this Vector2 v, Single target)
        {
            var l = v.Length();

            return new Vector2(
                x: (Single)Math.Cos(target) * l,
                y: (Single)Math.Sin(target) * l);
        }
    }
}
