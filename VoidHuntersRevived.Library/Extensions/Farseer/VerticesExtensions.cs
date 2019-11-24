using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Extensions.Farseer
{
    public static class VerticesExtensions
    {
        #region Transform Methods
        public static void Transform(this Vertices vertices, Matrix transform)
        {
            vertices.Transform(ref transform);
        }
        #endregion
    }
}
