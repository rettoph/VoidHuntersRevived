using Guppy.Extensions.System;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using tainicom.Aether.Physics2D.Common;

namespace VoidHuntersRevived.Library.Extensions.Aether
{
    public static class VerticesExtensions
    {
        public static Vertices Clone(this Vertices vertices)
            => new Vertices(vertices);

        public static Vertices Clone(this Vertices vertices, Matrix matrix)
           => vertices.Clone(ref matrix);

        public static Vertices Clone(this Vertices vertices, ref Matrix matrix)
        {
            Vertices clone = vertices.Clone();
            clone.Transform(ref matrix);

            return clone;
        }
    }
}
