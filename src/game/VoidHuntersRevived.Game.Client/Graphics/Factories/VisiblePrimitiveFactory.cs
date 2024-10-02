using Guppy.Core.Common.Attributes;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Graphics.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Components;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;
using VoidHuntersRevived.Domain.Graphics.Common.Utilities;
using VoidHuntersRevived.Domain.Graphics.Factories;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Static;
using VoidHuntersRevived.Domain.Pieces.Common.Graphics.Vertices;

namespace VoidHuntersRevived.Game.Client.Graphics.Factories
{
    [AutoLoad]
    internal sealed class VisiblePrimitiveFactory(IEntityTypeService entityTypeService, GraphicsDevice graphics) : IPrimitiveFactory
    {
        private readonly GraphicsDevice _graphics = graphics;
        private readonly IEntityTypeService _entityTypeService = entityTypeService;

        public IEnumerable<IPrimitive> BuildPrimitives()
        {
            List<IPrimitive> primitives = new List<IPrimitive>();

            foreach (IEntityType entityTypeProvider in _entityTypeService.GetAll())
            {
                if (entityTypeProvider.Components.TryGet<Visible>(out Visible visible) == false)
                {
                    continue;
                }

                primitives.Add(VisiblePrimitiveFactory.BuildVisiblePrimitive(
                    entityTypeKey: entityTypeProvider.Key,
                    sequence: entityTypeProvider.Components.Get<PrimitiveSequence>().Value,
                    visible: visible,
                    graphics: _graphics));
            }

            return primitives;
        }

        private static IPrimitive BuildVisiblePrimitive(
            Key<IEntityType> entityTypeKey,
            int sequence,
            Visible visible,
            GraphicsDevice graphics)
        {
            int count;
            short[] indexBuffer = new short[10];

            List<VertexStaticVisible> fillVertices = new List<VertexStaticVisible>();
            List<short> fillIndices = new List<short>();

            count = 0;
            for (int shape_i = 0; shape_i < visible.Fill.count; shape_i++)
            {
                Shape shape = visible.Fill[shape_i];

                indexBuffer[0] = (short)fillVertices.Count;
                fillVertices.Add(new VertexStaticVisible(shape.Vertices[0]));
                count++;

                indexBuffer[1] = (short)fillVertices.Count;
                fillVertices.Add(new VertexStaticVisible(shape.Vertices[1]));
                count++;

                for (int vertex_i = 2; vertex_i < shape.Vertices.count; vertex_i++)
                {
                    indexBuffer[2] = (short)fillVertices.Count;
                    fillVertices.Add(new VertexStaticVisible(shape.Vertices[vertex_i]));
                    count++;

                    fillIndices.AddRange(indexBuffer[..3]);
                    indexBuffer[1] = indexBuffer[2];
                }
            }

            List<VertexStaticVisible> traceVertices = new List<VertexStaticVisible>();
            List<short> traceIndices = new List<short>();

            count = 0;
            for (int shape_i = 0; shape_i < visible.Trace.count; shape_i++)
            {
                Shape shape = visible.Trace[shape_i];

                indexBuffer[0] = (short)traceVertices.Count;
                traceVertices.Add(new VertexStaticVisible(shape.Vertices[0], true));
                count++;

                for (int vertex_i = 1; vertex_i < shape.Vertices.count; vertex_i++)
                {
                    indexBuffer[1] = (short)traceVertices.Count;
                    traceVertices.Add(new VertexStaticVisible(shape.Vertices[vertex_i], true));
                    count++;

                    traceIndices.AddRange(indexBuffer[..2]);
                    indexBuffer[0] = indexBuffer[1];
                }
            }

            VertexBuffer fillBuffer = new VertexBuffer(graphics, typeof(VertexStaticVisible), fillVertices.Count, BufferUsage.WriteOnly);
            fillBuffer.SetData(fillVertices.ToArray());

            IndexBuffer fillIndexBuffer = new IndexBuffer(graphics, IndexElementSize.SixteenBits, fillIndices.Count, BufferUsage.WriteOnly);
            fillIndexBuffer.SetData(fillIndices.ToArray());

            VertexBuffer traceBuffer = new VertexBuffer(graphics, typeof(VertexStaticVisible), traceVertices.Count, BufferUsage.WriteOnly);
            traceBuffer.SetData(traceVertices.ToArray());

            IndexBuffer traceIndexBuffer = new IndexBuffer(graphics, IndexElementSize.SixteenBits, traceIndices.Count, BufferUsage.WriteOnly);
            traceIndexBuffer.SetData(traceIndices.ToArray());

            return new Primitive<VertexVisible>(
                graphics: graphics,
                entityTypeKey: entityTypeKey,
                primitiveGroupSequences: [
                    new PrimitiveGroupSequence(PrimitiveGroupEnum.Background, sequence),
                    new PrimitiveGroupSequence(PrimitiveGroupEnum.Foreground, sequence)
                ],
                bufferContexts: [
                    new BufferContext<VertexStaticVisible>(PrimitiveType.TriangleList, fillVertices.ToArray(), fillIndices.ToArray()),
                    new BufferContext<VertexStaticVisible>(PrimitiveType.LineList, traceVertices.ToArray(), traceIndices.ToArray()),
                ]);
        }
    }
}
