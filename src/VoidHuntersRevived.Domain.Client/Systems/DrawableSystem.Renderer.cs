using Guppy.MonoGame.Primitives;
using Guppy.MonoGame.Utilities.Cameras;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using VoidHuntersRevived.Common.Entities.ShipParts.Configurations;

namespace VoidHuntersRevived.Domain.Client.Systems
{
    internal partial class DrawableSystem
    {
        private sealed class Renderer
        {
            private readonly DrawConfiguration _configuration;
            private readonly PrimitiveBatch<VertexPositionColor> _primitiveBatch;
            private readonly Color _shapeColor;
            private readonly Color _pathColor;
            private readonly PrimitiveShape[] _shapes;
            private readonly PrimitiveShape[] _paths;

            public Renderer(
                Camera camera,
                PrimitiveBatch<VertexPositionColor> primitiveBatch,
                IResourceProvider resources,
                DrawConfiguration configuration)
            {
                _configuration = configuration;
                _primitiveBatch = primitiveBatch;
                _shapeColor = resources.Get<Color>(_configuration.Color).Value;
                _pathColor = Color.Lerp(resources.Get<Color>(_configuration.Color).Value, Color.White, 0.25f);
                _shapes = _configuration.Shapes.Select(x => new PrimitiveShape(x)).ToArray();
                _paths = _configuration.Paths.Select(x => new ProjectedShape(camera, x)).ToArray();
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
