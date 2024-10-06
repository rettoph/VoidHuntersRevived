using Guppy.Game.MonoGame.Common.Utilities.Cameras;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Domain.Graphics.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Contexts;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;
using VoidHuntersRevived.Domain.Graphics.Common.Utilities;
using VoidHuntersRevived.Domain.Pieces.Common.Graphics.Vertices;
using VoidHuntersRevived.Game.Client.Graphics.Effects;

namespace VoidHuntersRevived.Game.Client.Primitives
{
    public class VisiblePrimitive(
        PrimitiveContext context,
        PrimitiveSequenceGroupEnum sequenceGroup,
        GraphicsDevice graphics,
        BufferContext<VertexStaticVisible> staticBufferContext,
        VisibleEffect effect,
        Camera2D camera) : BaseEntityPrimitive<VertexVisible>(context, sequenceGroup, graphics, staticBufferContext, effect)
    {
        private readonly VisibleEffect _effect = effect;
        private readonly Camera2D _camera = camera;

        public override void Draw(IDrawPrimitiveContext context)
        {
            _effect.WorldViewProjection = _camera.World * _camera.View * _camera.Projection;

            base.Draw(context);
        }
    }
}
