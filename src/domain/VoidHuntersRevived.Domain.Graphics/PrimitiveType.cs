using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Domain.Graphics.Common;

namespace VoidHuntersRevived.Domain.Graphics
{
    public abstract class PrimitiveType : IPrimitiveType
    {
        private readonly Lazy<IPrimitive[]> _primitives;
        private bool _disposed = false;

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

        protected virtual void Dispose(bool disposing)
        {
            if (this._disposed == false)
            {
                if (disposing == true)
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

                this._disposed = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
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