using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;

namespace VoidHuntersRevived.Domain.Graphics.Common.Utilities
{
    public abstract class BufferContext
    {
        internal BufferContext()
        {

        }

        public abstract VertexBuffer[] BuildVertexBuffers(GraphicsDevice graphics);

        public abstract IndexBuffer[] BuildIndexBuffers(GraphicsDevice graphics);

        public abstract IEnumerable<PrimitiveTypeEnum> GetTypes();
    }

    public class BufferContext<TVertex> : BufferContext
        where TVertex : unmanaged, IVertexType
    {
        private readonly List<PrimitiveTypeEnum> _types = [];
        private readonly Dictionary<PrimitiveTypeEnum, List<TVertex>> _vertices = [];
        private readonly Dictionary<PrimitiveTypeEnum, List<short>> _indices = [];

        public void AddVertex(PrimitiveTypeEnum type, TVertex vertex, out short index)
        {
            this.GetCollections(type, out var vertices, out _);

            index = (short)vertices.Count;
            vertices.Add(vertex);
        }

        public void AddIndices(PrimitiveTypeEnum type, params short[] indices)
        {
            this.GetCollections(type, out var _, out var list);
            list.AddRange(indices);
        }

        public override IEnumerable<PrimitiveTypeEnum> GetTypes() => this._types;

        public override VertexBuffer[] BuildVertexBuffers(GraphicsDevice graphics)
        {
            List<VertexBuffer> buffers = [];

            foreach (PrimitiveTypeEnum type in this.GetTypes())
            {
                VertexBuffer buffer = new(graphics, typeof(TVertex), this._vertices[type].Count, BufferUsage.WriteOnly);
                buffer.SetData([.. this._vertices[type]]);
                buffers.Add(buffer);
            }

            return [.. buffers];
        }

        public override IndexBuffer[] BuildIndexBuffers(GraphicsDevice graphics)
        {
            List<IndexBuffer> buffers = [];

            foreach (PrimitiveTypeEnum type in this.GetTypes())
            {
                IndexBuffer buffer = new(graphics, IndexElementSize.SixteenBits, this._indices[type].Count, BufferUsage.WriteOnly);
                buffer.SetData([.. this._indices[type]]);
                buffers.Add(buffer);
            }

            return [.. buffers];
        }

        private void GetCollections(PrimitiveTypeEnum type, out List<TVertex> vertices, out List<short> indices)
        {
            ref List<TVertex> verticesRef = ref CollectionsMarshal.GetValueRefOrAddDefault(this._vertices, type, out bool exists)!;
            if (exists == true)
            {
                vertices = verticesRef;
                indices = this._indices[type];
                return;
            }

            verticesRef = [];

            vertices = verticesRef;
            indices = [];

            this._types.Add(type);
            this._indices.Add(type, indices);
        }
    }
}