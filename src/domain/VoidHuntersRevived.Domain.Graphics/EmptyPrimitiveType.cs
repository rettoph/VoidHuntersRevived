using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Domain.Graphics.Common;

namespace VoidHuntersRevived.Domain.Graphics
{

    public class EmptyPrimitiveType<TVertexInstance, TVertexStatic, TEffect> : IPrimitiveType<TVertexInstance, TVertexStatic, TEffect>
        where TVertexInstance : unmanaged, IVertexType
        where TVertexStatic : unmanaged, IVertexType
        where TEffect : Effect
    {
        public IPrimitive[] Primitives { get; } = [];

        public VertexBuffer VertexBuffer => throw new NotImplementedException();

        public IndexBuffer[] IndexBuffers { get; } = [];

        public PrimitiveTypeEnum[] BufferTypes { get; } = [];

        public void Dispose()
        {
            //
        }
    }
}
