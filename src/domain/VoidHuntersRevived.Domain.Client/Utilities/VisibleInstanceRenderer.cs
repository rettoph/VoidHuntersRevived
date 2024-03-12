using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.FixedPoint.Extensions;
using VoidHuntersRevived.Domain.Client.Graphics.Vertices;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;

namespace VoidHuntersRevived.Domain.Client.Utilities
{
    internal sealed class VisibleInstanceRenderer : IDisposable
    {
        private readonly GraphicsDevice _graphics;
        private readonly VertexBuffer _staticVertices;
        private readonly IndexBuffer _indices;

        private VertexBufferManager<VertexInstanceVisible> _instanceVertices;
        private VertexBufferBinding[] _bindings;

        public IndexBuffer Indices => _indices;

        public int StaticTriangleCount { get; }
        public int InstanceCount => _instanceVertices.Count;
        public int TriangleCount => this.StaticTriangleCount * this.InstanceCount;

        public VisibleInstanceRenderer(GraphicsDevice graphics, VertexBuffer staticVertices, IndexBuffer indices)
        {
            _graphics = graphics;
            _staticVertices = staticVertices;
            _indices = indices;
            _bindings = Array.Empty<VertexBufferBinding>();

            _instanceVertices = new VertexBufferManager<VertexInstanceVisible>(graphics, 100);

            this.StaticTriangleCount = _indices.IndexCount / 3;

            this.SetBindings();
        }

        public void Dispose()
        {
            _indices.Dispose();
            _staticVertices.Dispose();
            _instanceVertices.VertexBuffer.Dispose();
        }

        private void SetBindings()
        {
            _bindings = [
                new VertexBufferBinding(_staticVertices, 0, 0),
                new VertexBufferBinding(_instanceVertices.VertexBuffer, 0, 1)
            ];
        }

        public VertexBufferBinding[] SetInstanceData(IEntityService entities, int count, ref EntityFilterCollection instances)
        {
            if (_instanceVertices.Resize(count))
            {
                this.SetBindings();
            }

            int instanceVertexIndex = 0;
            foreach (var (indices, group) in instances)
            {
                var (statuses, nodes, colorSchemes, _) = entities.QueryEntities<EntityStatus, Node, ColorScheme>(group);

                for (int i = 0; i < indices.count; i++)
                {
                    uint index = indices[i];
                    if (statuses[index].IsDespawned)
                    {
                        continue;
                    }

                    ref Node node = ref nodes[index];
                    ref ColorScheme colorScheme = ref colorSchemes[index];

                    ref VertexInstanceVisible instanceVertex = ref _instanceVertices.Data[instanceVertexIndex++];

                    instanceVertex.InstanceTranformation = node.Transformation.ToTransformationXnaMatrix();
                    instanceVertex.PrimaryColor = colorScheme.Primary.Current.PackedValue;
                }
            }

            _instanceVertices.SetData(instanceVertexIndex);

            return _bindings;
        }
    }
}
