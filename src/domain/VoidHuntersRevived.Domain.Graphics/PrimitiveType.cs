using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Domain.Graphics.Common;

namespace VoidHuntersRevived.Domain.Graphics
{
    public abstract class PrimitiveType : IPrimitiveType
    {
        private readonly Lazy<IPrimitive[]> _primitives;

        public IPrimitive[] Primitives => this._primitives.Value;
        public VertexBuffer VertexBuffer { get; }
        public IndexBuffer[] IndexBuffers { get; }
        public PrimitiveTypeEnum[] BufferTypes { get; }

        internal PrimitiveType(Lazy<IPrimitive[]> primitives, VertexBuffer vertexBuffer, IndexBuffer[] indexBuffers, PrimitiveTypeEnum[] bufferTypes)
        {
            this._primitives = primitives;
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

            foreach (IPrimitive primitive in this.Primitives)
            {
                primitive.Dispose();
            }
        }
    }

    public abstract class PrimitiveType<TVertexInstance> : PrimitiveType, IPrimitiveType<TVertexInstance>
        where TVertexInstance : unmanaged, IVertexType
    {
        internal PrimitiveType(Lazy<IPrimitive[]> primitives, VertexBuffer vertexBuffer, IndexBuffer[] indexBuffers, PrimitiveTypeEnum[] bufferTypes) : base(primitives, vertexBuffer, indexBuffers, bufferTypes)
        {
        }
    }

    public class PrimitiveType<TVertexInstance, TVertexStatic, TEffect>(Lazy<IPrimitive[]> primitives, VertexBuffer vertexBuffer, IndexBuffer[] indexBuffers, PrimitiveTypeEnum[] bufferTypes) : PrimitiveType<TVertexInstance>(primitives, vertexBuffer, indexBuffers, bufferTypes), IPrimitiveType<TVertexInstance, TVertexStatic, TEffect>
        where TVertexInstance : unmanaged, IVertexType
        where TVertexStatic : unmanaged, IVertexType
        where TEffect : Effect
    {
    }
}