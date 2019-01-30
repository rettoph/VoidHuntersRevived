using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Helpers
{
    public static class Vector2Helper
    {
        public static Vector2 FromThetaDistance(float theta, float distance)
        {
            return new Vector2((float)Math.Cos(theta) * distance, (float)Math.Sin(theta) * distance);
        }
    }
}
