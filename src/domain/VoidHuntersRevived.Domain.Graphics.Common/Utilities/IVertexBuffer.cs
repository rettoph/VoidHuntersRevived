using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;

namespace VoidHuntersRevived.Domain.Graphics.Common.Utilities
{
    public interface IVertexBuffer
    {
        IKey<IEntityType>? EntityTypeKey { get; }

        PrimitiveGroupEnum Group { get; }

        /// <summary>
        /// The position to render the vertex buffer relative to other buffers in the same group
        /// </summary>
        int Sequence { get; }

        Type VertexType { get; }

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
