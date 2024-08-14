using Microsoft.Xna.Framework.Graphics;

namespace VoidHuntersRevived.Domain.Graphics.Common.Providers
{
    public interface IVertexProvider
    {
        void Clear();

        void Flush();

        void EnsureFit(int size);
    }

    public interface IVertexProvider<TVertex> : IVertexProvider
        where TVertex : unmanaged, IVertexType
    {
        void SetNextVertexUnsafe(TVertex vertex);

        ref TVertex GetNextVertexUnsafe();

        void SetNextVertex(TVertex vertex);

        ref TVertex GetNextVertex();
    }
}
