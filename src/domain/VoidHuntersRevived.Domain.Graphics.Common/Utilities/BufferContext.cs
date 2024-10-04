using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace VoidHuntersRevived.Domain.Graphics.Common.Utilities
{
    public abstract class BufferContext
    {
        internal BufferContext()
        {

        }

        public abstract VertexBuffer[] BuildVertexBuffers(GraphicsDevice graphics);

        public abstract IndexBuffer[] BuildIndexBuffers(GraphicsDevice graphics);

        public abstract IEnumerable<PrimitiveType> GetTypes();
    }

    public class BufferContext<TVertex> : BufferContext
        where TVertex : unmanaged, IVertexType
    {
        private readonly List<PrimitiveType> _types = [];
        private readonly Dictionary<PrimitiveType, List<TVertex>> _vertices = [];
        private readonly Dictionary<PrimitiveType, List<short>> _indices = [];

        public void AddVertex(PrimitiveType type, TVertex vertex, out short index)
        {
            this.GetCollections(type, out var vertices, out _);

            index = (short)vertices.Count;
            vertices.Add(vertex);
        }

        public void AddIndices(PrimitiveType type, params short[] indices)
        {
            this.GetCollections(type, out var _, out var list);
            list.AddRange(indices);
        }

        public override IEnumerable<PrimitiveType> GetTypes() => _types;

        public override VertexBuffer[] BuildVertexBuffers(GraphicsDevice graphics)
        {
            List<VertexBuffer> buffers = [];

            foreach (PrimitiveType type in this.GetTypes())
            {
                VertexBuffer buffer = new(graphics, typeof(TVertex), _vertices[type].Count, BufferUsage.WriteOnly);
                buffer.SetData([.. _vertices[type]]);
                buffers.Add(buffer);
            }

            return [.. buffers];
        }

        public override IndexBuffer[] BuildIndexBuffers(GraphicsDevice graphics)
        {
            List<IndexBuffer> buffers = [];

            foreach (PrimitiveType type in this.GetTypes())
            {
                IndexBuffer buffer = new(graphics, IndexElementSize.SixteenBits, _indices[type].Count, BufferUsage.WriteOnly);
                buffer.SetData([.. _indices[type]]);
                buffers.Add(buffer);
            }

            return [.. buffers];
        }

        private void GetCollections(PrimitiveType type, out List<TVertex> vertices, out List<short> indices)
        {
            ref List<TVertex> verticesRef = ref CollectionsMarshal.GetValueRefOrAddDefault(_vertices, type, out bool exists)!;
            if (exists == true)
            {
                vertices = verticesRef;
                indices = _indices[type];
                return;
            }

            verticesRef = [];

            vertices = verticesRef;
            indices = [];

            _types.Add(type);
            _indices.Add(type, indices);
        }
    }
}
