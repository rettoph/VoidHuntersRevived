using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Graphics.Common.Utilities
{
    public interface IVertexBuffer
    {
        EntitiesDB EntitiesDb { set; }
        int InstanceCount { get; }

        void Clear();

        void Flush();

        void Draw(Effect effect);

        void EnsureFit(int size);

        ref EntityFilterCollection GetFilter<TComponent>()
            where TComponent : unmanaged, IVertexType, IEntityComponent;
    }

    public interface IVertexBuffer<TVertex> : IVertexBuffer
        where TVertex : unmanaged, IVertexType
    {
        void SetNextVertexUnsafe(TVertex vertex);

        ref TVertex GetNextVertexUnsafe();

        void SetNextVertex(TVertex vertex);

        ref TVertex GetNextVertex();
    }
}
