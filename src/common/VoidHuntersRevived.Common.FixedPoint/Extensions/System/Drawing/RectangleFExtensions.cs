using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.FixedPoint.FixedPoint;

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
