using Microsoft.Xna.Framework;
using tainicom.Aether.Physics2D.Common;

namespace VoidHuntersRevived.Library.Extensions.Aether
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
