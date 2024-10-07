using Guppy.Core.Common.Attributes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;

namespace VoidHuntersRevived.Domain.Graphics.Common
{
    public interface IPrimitive : IDisposable
    {
        Type VertexType { get; }
        int Sequence { get; }
        VertexBuffer StaticVertexBuffer { get; }
        IndexBuffer[] StaticIndexBuffers { get; }
        PrimitiveTypeEnum[] BufferTypes { get; }
        int[] StaticPrimitiveCount { get; }
        GraphicsDevice Graphics { get; }
        PrimitiveSequenceGroupEnum SequenceGroup { get; }
        int InstanceCount { get; }
        VertexBuffer InstanceVertexBuffer { get; }
        VertexBufferBinding[][] VertexBufferBindings { get; }
        CombinedFilterID CombinedFilterId { get; }
    }

    public interface IPrimitive<TVertexInstance> : IPrimitive
        where TVertexInstance : unmanaged, IVertexType
    {
        TVertexInstance[] InstanceVertices { get; }

        void EnsureFit(int size);

        void SetNextVertexUnsafe(TVertexInstance vertex);

        ref TVertexInstance GetNextVertexUnsafe();

        void SetNextVertex(TVertexInstance vertex);

        ref TVertexInstance GetNextVertex();

        bool Flush();

        void Clear();

        [RequireSequenceGroup<PrimitiveSequenceGroupEnum>]
        void Draw(GameTime gameTime);
    }

    public interface IPrimitive<TVertexInstance, TVertexStatic, TEffect> : IPrimitive<TVertexInstance>
        where TVertexInstance : unmanaged, IVertexType
        where TVertexStatic : unmanaged, IVertexType
        where TEffect : Effect
    {
        TEffect Effect { get; }
    }
}
