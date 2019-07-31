using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Utilities
{
    public static class RadHelper
    {
        public static Single FromDeg(Single deg)
        {
            return deg * (MathHelper.Pi / 180);
        }
    }
}
