using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;

namespace System.Drawing
{
    public static class RectangleFExtensions
    {
        public static FixRectangle ToFixRectangle(this RectangleF rect)
        {
            return new FixRectangle(
                x: (Fix64)rect.X,
                y: (Fix64)rect.Y,
                width: (Fix64)rect.Width,
                height: (Fix64)rect.Height);
        }
    }
}
