using FarseerPhysics;
using FarseerPhysics.Common;
using Guppy.Extensions.Collection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Client.Textures.Extensions.Drawing;

namespace VoidHuntersRevived.Client.Textures.Extensions.Farseer
{
    public static class VerticesExtensions
    {
        /// <summary>
        /// Convert the vertices into dimension sizes
        /// vertices with points all aboce 0, 0
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyCollection<Vertices> ToImageDimensions(this IReadOnlyCollection<Vertices> inputs)
        {
            var output = inputs.Select(v => new Vertices(v)).ToList();

            // Scale all the internal vertice points by the display ratio
            var scale = Matrix.CreateScale(ConvertUnits.DisplayUnitsToSimUnitsRatio);
            output.ForEach(v => v.Transform(ref scale));

            // Find the smallest points
            Vector2 min = new Vector2(
                x: output[0].Min(p => p.X),
                y: output[0].Min(p => p.Y));
            output.ForEach(v =>
            {
                min.X = Math.Min(min.X, v.Min(p => p.X));
                min.Y = Math.Min(min.Y, v.Min(p => p.Y));
            });

            // Translate the vertices by the smallest points
            output.ForEach(v => v.Translate(-min));

            return output;
        }

        /// <summary>
        /// Convert the vertices into dimension sizes
        /// vertices with points all aboce 0, 0
        /// </summary>
        /// <returns></returns>
        public static Vertices ToImageDimensions(this Vertices input)
        {
            var inputs = new List<Vertices>();
            inputs.Add(input);

            return inputs.ToImageDimensions().First();
        }

        public static Image DrawShape(this IReadOnlyCollection<Vertices> vertices, Pen pen, Brush brush)
        {
            var image = new Bitmap((Int32)vertices.SelectMany(v => v).Max(v => Math.Round(v.X + pen.Width)), (Int32)vertices.SelectMany(v => v).Max(v => Math.Round(v.Y + pen.Width)));
            var graphics = Graphics.FromImage(image);

            vertices.ForEach(v => {
                v.Translate(Vector2.One * pen.Width / 2);
                graphics.DrawShape(v, pen, brush);
            });

            return image;
        }

        public static Image DrawShape(this Vertices vertices, Pen pen, Brush brush)
        {
            var v = new List<Vertices>();
            v.Add(vertices);

            return v.DrawShape(pen, brush);
        }
    }
}
