using Microsoft.Xna.Framework.Graphics;

namespace VoidHuntersRevived.Domain.Graphics.Common
{
    public abstract class PrimitiveType : IDisposable
    {
        public readonly Lazy<Primitive[]> Primitives;
        public readonly VertexBuffer VertexBuffer;
        public readonly IndexBuffer[] IndexBuffers;
        public readonly PrimitiveTypeEnum[] BufferTypes;

        internal PrimitiveType(Lazy<Primitive[]> primitives, VertexBuffer vertexBuffer, IndexBuffer[] indexBuffers, PrimitiveTypeEnum[] bufferTypes)
        {
            this.Primitives = primitives;
            this.VertexBuffer = vertexBuffer;
            this.IndexBuffers = indexBuffers;
            this.BufferTypes = bufferTypes;
        }

        public void Dispose()
        {
            this.VertexBuffer.Dispose();
            foreach (IndexBuffer indexBuffer in this.IndexBuffers)
            {
                indexBuffer.Dispose();
            }
        }
    }

    public abstract class PrimitiveType<TVertexInstance> : PrimitiveType
        where TVertexInstance : unmanaged, IVertexType
    {
        internal PrimitiveType(Lazy<Primitive[]> primitives, VertexBuffer vertexBuffer, IndexBuffer[] indexBuffers, PrimitiveTypeEnum[] bufferTypes) : base(primitives, vertexBuffer, indexBuffers, bufferTypes)
        {
        }
    }

    public class PrimitiveType<TVertexInstance, TVertexStatic, TEffect> : PrimitiveType<TVertexInstance>
        where TVertexInstance : unmanaged, IVertexType
        where TVertexStatic : unmanaged, IVertexType
        where TEffect : Effect
    {
        public PrimitiveType(Lazy<Primitive[]> primitives, VertexBuffer vertexBuffer, IndexBuffer[] indexBuffers, PrimitiveTypeEnum[] bufferTypes) : base(primitives, vertexBuffer, indexBuffers, bufferTypes)
        {
        }
    }
}
