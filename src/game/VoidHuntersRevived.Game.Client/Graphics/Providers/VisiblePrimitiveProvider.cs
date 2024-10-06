using Guppy.Core.Common.Attributes;
using Guppy.Core.Resources.Common;
using Guppy.Core.Resources.Common.Services;
using Guppy.Game.MonoGame.Common.Utilities.Cameras;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Domain.Graphics.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;
using VoidHuntersRevived.Domain.Graphics.Common.Providers;
using VoidHuntersRevived.Domain.Graphics.Common.Utilities;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Graphics.Vertices;
using VoidHuntersRevived.Domain.Pieces.Common.Resources;
using VoidHuntersRevived.Game.Client.Graphics.Effects;
using VoidHuntersRevived.Game.Client.Primitives;

namespace VoidHuntersRevived.Game.Client.Graphics.Providers
{
    [AutoLoad]
    public class VisiblePrimitiveProvider(
        IResourceService resourceService,
        GraphicsDevice graphics,
        VisibleEffect effect,
        Camera2D camera) : IPrimitiveProvider
    {
        private readonly IPrimitive[] _primitives = VisiblePrimitiveProvider.BuildVisiblePrimitives(resourceService, graphics, effect, camera);

        public IEnumerable<IPrimitive> GetPrimitives() => _primitives;

        private static readonly PrimitiveSequenceGroupEnum[] VisibleSequenceGroups = [PrimitiveSequenceGroupEnum.Background, PrimitiveSequenceGroupEnum.Foreground];
        private static IPrimitive[] BuildVisiblePrimitives(
            IResourceService resourceService,
            GraphicsDevice graphics,
            VisibleEffect effect,
            Camera2D camera)
        {
            List<IPrimitive> primitives = [];

            foreach (ResourceValue<Visible> visible in resourceService.GetValues<Visible>())
            {
                primitives.AddRange(VisiblePrimitiveProvider.BuildVisiblePrimitivesBySequenceGroups(
                    visible: visible.Value,
                    graphics: graphics,
                    effect: effect,
                    camera: camera));
            }

            return [.. primitives];
        }

        private static IEnumerable<IPrimitive<VertexVisible>> BuildVisiblePrimitivesBySequenceGroups(
            Visible visible,
            GraphicsDevice graphics,
            VisibleEffect effect,
            Camera2D camera)
        {
            short[] indexBuffer = new short[10];
            BufferContext<VertexStaticVisible> staticBufferContext = new();
            for (int shape_i = 0; shape_i < visible.Fill.Length; shape_i++)
            {
                Shape shape = visible.Fill[shape_i];

                staticBufferContext.AddVertex(PrimitiveType.TriangleList, new VertexStaticVisible(shape.Vertices[0]), out indexBuffer[0]);
                staticBufferContext.AddVertex(PrimitiveType.TriangleList, new VertexStaticVisible(shape.Vertices[1]), out indexBuffer[1]);

                for (int vertex_i = 2; vertex_i < shape.Vertices.Length; vertex_i++)
                {
                    staticBufferContext.AddVertex(PrimitiveType.TriangleList, new VertexStaticVisible(shape.Vertices[vertex_i]), out indexBuffer[2]);
                    staticBufferContext.AddIndices(PrimitiveType.TriangleList, indexBuffer[..3]);

                    indexBuffer[1] = indexBuffer[2];
                }
            }

            for (int shape_i = 0; shape_i < visible.Trace.Length; shape_i++)
            {
                Shape shape = visible.Trace[shape_i];

                staticBufferContext.AddVertex(PrimitiveType.LineList, new VertexStaticVisible(shape.Vertices[0]), out indexBuffer[0]);

                for (int vertex_i = 1; vertex_i < shape.Vertices.Length; vertex_i++)
                {
                    staticBufferContext.AddVertex(PrimitiveType.LineList, new VertexStaticVisible(shape.Vertices[vertex_i]), out indexBuffer[1]);
                    staticBufferContext.AddIndices(PrimitiveType.LineList, indexBuffer[..2]);

                    indexBuffer[0] = indexBuffer[1];
                }
            }

            foreach (PrimitiveSequenceGroupEnum sequenceGroup in VisibleSequenceGroups)
            {
                yield return new VisiblePrimitive(
                    context: visible.Primitive,
                    sequenceGroup: sequenceGroup,
                    graphics: graphics,
                    staticBufferContext: staticBufferContext,
                    effect: effect,
                    camera: camera);
            }
        }
    }
}
