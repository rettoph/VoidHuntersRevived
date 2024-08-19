using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Graphics.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;
using VoidHuntersRevived.Domain.Graphics.Common.Utilities;

namespace VoidHuntersRevived.Domain.Graphics.Utilities
{
    public class VertexTypeManager<TVertex> : IVertexTypeManager<TVertex>
        where TVertex : unmanaged, IVertexType
    {
        public Type VertexType => typeof(TVertex);

        public PrimitiveGroupEnum Group { get; }

        public CombinedFilterID FilterId { get; }

        public IVertexBuffer<TVertex>[] VertexBuffers { get; }

        IVertexBuffer[] IVertexTypeManager.VertexBuffers => VertexBuffers;

        public IPrimitive<TVertex>[] Primitives { get; }

        IPrimitive[] IVertexTypeManager.Primitives => this.Primitives;

        public VertexTypeManager(
            PrimitiveGroupEnum group,
            CombinedFilterID filterId,
            IPrimitive<TVertex>[] primitives)
        {
            this.Group = group;
            this.FilterId = filterId;
            this.Primitives = primitives;
            this.VertexBuffers = primitives.Select(x => x.GetVertexBuffer(this.Group)).ToArray();
        }
    }
}
