using Guppy.MonoGame.Primitives;
using Guppy.MonoGame.Utilities.Cameras;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;

namespace VoidHuntersRevived.Domain.Client.Systems
{
    internal partial class DrawableShipPartSystem
    {
        private sealed class Renderer
        {
            private readonly Drawable _drawable;
            private readonly PrimitiveBatch<VertexPositionColor> _primitiveBatch;
            private readonly Color _shapeColor;
            private readonly Color _pathColor;
            private readonly PrimitiveShape[] _shapes;
            private readonly PrimitiveShape[] _paths;

            public Renderer(
                Camera camera,
                PrimitiveBatch<VertexPositionColor> primitiveBatch,
                IResourceProvider resources,
                Drawable drawable)
            {
                _drawable = drawable;
                _primitiveBatch = primitiveBatch;
                _shapeColor = resources.Get<Color>(_drawable.Color).Value;
                _pathColor = Color.Lerp(resources.Get<Color>(_drawable.Color).Value, Color.White, 0.25f);
                _shapes = _drawable.Shapes.Select(x => new PrimitiveShape(x)).ToArray();
                _paths = _drawable.Paths.Select(x => new ProjectedShape(camera, x)).ToArray();
            }

            public void RenderShapes(Matrix transformation)
            {
                foreach(PrimitiveShape shape in _shapes)
                {
                    _primitiveBatch.Fill(shape, in _shapeColor, ref transformation);
                }
            }

            public void RenderPaths(Matrix transformation)
            {
                foreach (PrimitiveShape path in _paths)
                {
                    _primitiveBatch.Trace(path, in _pathColor, ref transformation);
                }
            }
        }
    }
}
