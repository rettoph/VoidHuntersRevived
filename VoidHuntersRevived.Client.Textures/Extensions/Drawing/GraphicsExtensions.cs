using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Client.Textures.Extensions.Drawing
{
    public static class GraphicsExtensions
    {
        public static void DrawShape(this Graphics graphics, Vertices vertices, Pen pen, Brush brush)
        {
            var p = vertices.Select(v => new PointF((float)Math.Round(v.X), (float)Math.Round(v.Y))).ToArray();

            graphics.FillPolygon(brush, p);
            graphics.DrawPolygon(pen, p);
        }
    }
}
