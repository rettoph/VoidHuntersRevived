using FarseerPhysics;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Client.Textures.Extensions.Farseer
{
    public static class VerticeExtensions
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
    }
}
