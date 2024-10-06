using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;

namespace VoidHuntersRevived.Domain.Graphics.Common
{
    public interface IPrimitive : IRuntimeSequenceGroup<PrimitiveSequenceGroupEnum>, IRuntimeSequence<PrimitiveSequenceGroupEnum>
    {
        Key<IPrimitive> Type { get; }

        PrimitiveSequenceGroupEnum SequenceGroup { get; }

        /// <summary>
        /// The position to render the vertex buffer relative to other buffers in the same group
        /// </summary>
        int Sequence { get; }

        Type VertexType { get; }

        int InstanceCount { get; }

        [RequireSequenceGroup<PrimitiveSequenceGroupEnum>]
        void Draw(IDrawPrimitiveContext context);

        void EnsureFit(int size);

        ref EntityFilterCollection GetFilter<TComponent>(EntitiesDB entitiesDb)
            where TComponent : unmanaged, IVertexType, IEntityComponent;
    }

    public interface IPrimitive<TVertex> : IPrimitive
        where TVertex : unmanaged, IVertexType
    {
        void SetNextVertexUnsafe(TVertex vertex);

        ref TVertex GetNextVertexUnsafe();

        void SetNextVertex(TVertex vertex);

        ref TVertex GetNextVertex();
    }
}
