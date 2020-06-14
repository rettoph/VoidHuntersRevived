using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace VoidHuntersRevived.Library.Extensions.System.Drawing
{
    public static class RectangleFExtensions
    {
        public static RectangleF Intersection(this RectangleF r1, RectangleF r2)
        {
            return new RectangleF()
            {
                X = Math.Max(r1.Left, r2.Left),
                Y = Math.Max(r1.Top, r2.Top),
                Width = Math.Min(r1.Right, r2.Right) - Math.Max(r1.Left, r2.Left),
                Height = Math.Min(r1.Bottom, r2.Bottom) - Math.Max(r1.Top, r2.Top)
            };
        }
    }
}
