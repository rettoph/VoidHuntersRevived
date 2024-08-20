using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;

namespace VoidHuntersRevived.Domain.Graphics.Common.Utilities
{
    public interface IVertexTypeManager
    {
        Type VertexType { get; }
        PrimitiveGroupEnum Group { get; }
        CombinedFilterID FilterId { get; }
        IPrimitive[] Primitives { get; }
        IVertexBuffer[] VertexBuffers { get; }
    }

    public interface IVertexTypeManager<TVertex> : IVertexTypeManager
        where TVertex : unmanaged, IVertexType
    {
        new IPrimitive<TVertex>[] Primitives { get; }
        new IVertexBuffer<TVertex>[] VertexBuffers { get; }
    }
}
