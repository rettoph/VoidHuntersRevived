using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;
using VoidHuntersRevived.Domain.Graphics.Common.Utilities;

namespace VoidHuntersRevived.Domain.Graphics.Common.Providers
{
    public interface IVertexTypeManagerProvider
    {
        Type VertexType { get; }
        PrimitiveGroupEnum[] Groups { get; }
        IPrimitive[] Primitives { get; }

        IVertexTypeManager GetByGroup(PrimitiveGroupEnum group);

        IEnumerable<IVertexTypeManager> GetAll();
    }

    public interface IVertexTypeManagerProvider<TVertex> : IVertexTypeManagerProvider
        where TVertex : unmanaged, IVertexType
    {
        new IPrimitive<TVertex>[] Primitives { get; }
        new IVertexTypeManager<TVertex> GetByGroup(PrimitiveGroupEnum group);

        new IEnumerable<IVertexTypeManager<TVertex>> GetAll();
    }
}
