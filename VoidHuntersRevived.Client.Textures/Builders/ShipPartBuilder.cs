using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guppy.Configurations;
using Guppy.Extensions.Collection;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Client.Textures.Attributes;
using VoidHuntersRevived.Client.Textures.Extensions.Farseer;
using VoidHuntersRevived.Library.Configurations;
using VoidHuntersRevived.Library.Entities.ShipParts;
using Color = System.Drawing.Color;

namespace VoidHuntersRevived.Client.Textures.Builders
{
    /// <summary>
    /// Simple class used to generate textures for ShipParts
    /// </summary>
    [IsBuilder(typeof(ShipPart))]
    class ShipPartBuilder : Builder
    {
        public static Pen Pen { get; set; } = new Pen(Color.FromArgb(150, 150, 150, 150), 4);
        public static Brush Brush { get; set; } = new SolidBrush(Color.FromArgb(150, 255, 255, 255));

        protected override Image Build(EntityConfiguration entity)
        {
            var config = entity.Data as ShipPartConfiguration;
            var vertices = config.Vertices.ToImageDimensions();
            var image = new Bitmap((Int32)vertices.SelectMany(v => v).Max(v => Math.Ceiling(v.X + ShipPartBuilder.Pen.Width)), (Int32)vertices.SelectMany(v => v).Max(v => Math.Ceiling(v.Y + ShipPartBuilder.Pen.Width)));
            var graphics = Graphics.FromImage(image);

            
            vertices.Select(v =>
            {
                v.Translate(Vector2.One * ShipPartBuilder.Pen.Width / 2);
                var points = new List<PointF>();
                points.AddRange(v.Select(p => new PointF(p.X, p.Y)));
                return points.ToArray();
            }).ForEach(p =>
            {
                graphics.FillPolygon(ShipPartBuilder.Brush, p);
                graphics.DrawPolygon(ShipPartBuilder.Pen, p);
            });

            return image;
        }
    }
}
