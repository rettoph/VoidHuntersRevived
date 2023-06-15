using Guppy.MonoGame.Primitives;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Game.Pieces.Properties;
using Guppy.MonoGame.Utilities.Cameras;

namespace VoidHuntersRevived.Game.Pieces.Utilities
{
    public sealed class VisibleRenderer
    {
        private readonly Visible _visible;
        private readonly PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private readonly Color _shapeColor;
        private readonly Color _pathColor;
        private readonly PrimitiveShape[] _shapes;
        private readonly PrimitiveShape[] _paths;

        public VisibleRenderer(
            Camera camera,
            PrimitiveBatch<VertexPositionColor> primitiveBatch,
            IResourceProvider resources,
            Visible drawable,
            Color? tint)
        {
            _visible = drawable;
            _primitiveBatch = primitiveBatch;
            _shapes = _visible.Shapes.Select(x => new PrimitiveShape(x)).ToArray();
            _paths = _visible.Paths.Select(x => new ProjectedShape(camera, x)).ToArray();

            _shapeColor = resources.Get<Color>(_visible.Color).Value;
            _pathColor = Color.Lerp(resources.Get<Color>(_visible.Color).Value, Color.White, 0.25f);

            if (tint is not null)
            {
                _shapeColor = Color.Lerp(_shapeColor, tint.Value, 0.5f);
                _pathColor = Color.Lerp(_pathColor, tint.Value, 0.5f);
            }
        }

        public void RenderShapes(Matrix transformation)
        {
            foreach (PrimitiveShape shape in _shapes)
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
