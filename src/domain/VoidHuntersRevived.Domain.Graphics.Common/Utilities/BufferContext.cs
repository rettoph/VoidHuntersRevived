using Microsoft.Xna.Framework.Graphics;

namespace VoidHuntersRevived.Domain.Graphics.Common.Utilities
{
    public abstract class BufferContext
    {
        public abstract PrimitiveType PrimitiveType { get; }

        internal BufferContext()
        {

        }

        public abstract VertexBuffer BuildVertexBuffer(GraphicsDevice graphics);

        public abstract IndexBuffer BuildIndexBuffer(GraphicsDevice graphics);
    }

    public class BufferContext<TVertex>(PrimitiveType primitiveType, TVertex[] vertices, short[] indices) : BufferContext
        where TVertex : unmanaged, IVertexType
    {
        private readonly TVertex[] _vertices = vertices;
        private readonly short[] _indices = indices;

        public override PrimitiveType PrimitiveType { get; } = primitiveType;

        public override VertexBuffer BuildVertexBuffer(GraphicsDevice graphics)
        {
            VertexBuffer buffer = new VertexBuffer(graphics, typeof(TVertex), _vertices.Length, BufferUsage.WriteOnly);
            buffer.SetData(_vertices);

            return buffer;
        }

        public override IndexBuffer BuildIndexBuffer(GraphicsDevice graphics)
        {
            IndexBuffer buffer = new IndexBuffer(graphics, IndexElementSize.SixteenBits, _indices.Length, BufferUsage.WriteOnly);
            buffer.SetData(_indices);

            return buffer;
        }
    }
}
