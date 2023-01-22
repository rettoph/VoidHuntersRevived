using Guppy.MonoGame.Primitives;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Common.Entities.ShipParts.Configurations;

namespace VoidHuntersRevived.Domain.Client.Systems
{
    internal partial class DrawableSystem
    {
        private sealed class Renderer
        {
            private readonly DrawConfiguration _configuration;
            private readonly PrimitiveBatch<VertexPositionColor> _primitiveBatch;
            private readonly Color _color;
            private readonly PrimitiveShape[] _shapes;

            public Renderer(
                PrimitiveBatch<VertexPositionColor> primitiveBatch,
                IResourceProvider resources,
                DrawConfiguration configuration)
            {
                _configuration = configuration;
                _primitiveBatch = primitiveBatch;
                _color = resources.Get<Color>(_configuration.Color);
                _shapes = _configuration.Shapes.Select(x => new PrimitiveShape(x)).ToArray();
            }

            public void Render(Matrix transformation)
            {
                foreach(PrimitiveShape shape in _shapes)
                {
                    _primitiveBatch.Fill(shape, in _color, ref transformation);
                }
            }
        }
    }
}
