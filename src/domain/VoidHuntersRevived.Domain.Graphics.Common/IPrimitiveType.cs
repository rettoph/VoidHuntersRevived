using Microsoft.Xna.Framework.Graphics;

namespace VoidHuntersRevived.Domain.Graphics.Common
{
    public interface IPrimitiveType : IDisposable
    {
        public IPrimitive[] Primitives { get; }
        public VertexBuffer VertexBuffer { get; }
        public IndexBuffer[] IndexBuffers { get; }
        public PrimitiveTypeEnum[] BufferTypes { get; }
    }

    public interface IPrimitiveType<TVertexInstance> : IPrimitiveType
        where TVertexInstance : unmanaged, IVertexType
    {
    }

    public interface IPrimitiveType<TVertexInstance, TVertexStatic, TEffect> : IPrimitiveType<TVertexInstance>
        where TVertexInstance : unmanaged, IVertexType
        where TVertexStatic : unmanaged, IVertexType
        where TEffect : Effect
    {
    }
}