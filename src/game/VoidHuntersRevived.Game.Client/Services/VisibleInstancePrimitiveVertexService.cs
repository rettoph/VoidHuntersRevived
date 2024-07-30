using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Static;
using VoidHuntersRevived.Game.Client.Common.Graphics.Vertices;
using VoidHuntersRevived.Game.Client.Common.Services;
using VoidHuntersRevived.Game.Client.Common.Utilities;

namespace VoidHuntersRevived.Game.Client.Services
{
    internal sealed class VisibleInstancePrimitiveVertexService : IVisibleInstanceVertexService, IDisposable
    {
        private readonly IEntityQueryService _entityQueryService;
        private readonly GraphicsDevice _graphics;
        private readonly Dictionary<Id<IEntityType>, InstanceVertexProvider<VertexInstanceVisible>> _providers;

        public VisibleInstancePrimitiveVertexService(IEntityQueryService entityQueryService, GraphicsDevice grapphics)
        {
            _entityQueryService = entityQueryService;
            _graphics = grapphics;
            _providers = new Dictionary<Id<IEntityType>, InstanceVertexProvider<VertexInstanceVisible>>();
        }

        public void Initialize()
        {
            foreach (var ((typeEntities, visibles, zIndices, count), _) in _entityQueryService.QueryEntities<TypeEntity, Visible>())
            {
                for (int i = 0; i < count; i++)
                {
                    _providers.Add(typeEntities[i].TypeId, VisibleInstancePrimitiveVertexService.BuildVisibleInstanceVertexProvider(typeEntities[i].TypeId, visibles[i], _graphics));
                }
            }
        }

        public void Dispose()
        {
            foreach (InstanceVertexProvider<VertexInstanceVisible> provider in _providers.Values)
            {
                provider.Dispose();
            }
        }

        public InstanceVertexProvider<VertexInstanceVisible> GetInstanceVertexProviderById(Id<IEntityType> id)
        {
            return _providers[id];
        }

        public IEnumerable<InstanceVertexProvider<VertexInstanceVisible>> GetAllInstanceVertexProviders()
        {
            return _providers.Values;
        }

        private static InstanceVertexProvider<VertexInstanceVisible> BuildVisibleInstanceVertexProvider(
            Id<IEntityType> entityType,
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

            return new InstanceVertexProvider<VertexInstanceVisible>(graphics, [fillBuffer, traceBuffer], [fillIndexBuffer, traceIndexBuffer], [PrimitiveType.TriangleList, PrimitiveType.LineList]);
        }
    }
}
