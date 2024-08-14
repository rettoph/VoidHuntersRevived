using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;
using VoidHuntersRevived.Domain.Graphics.Common.Providers;

namespace VoidHuntersRevived.Domain.Graphics.Common
{
    public interface IPrimitive
    {
        IKey<IEntityType> EntityTypeKey { get; }
        Type VertexType { get; }
    }

    public interface IPrimitive<TVertex> : IPrimitive
        where TVertex : unmanaged, IVertexType
    {
        IVertexProvider<TVertex> GetVertexProvider(PrimitiveGroupEnum primitiveGroup);
        IEnumerable<KeyValuePair<PrimitiveGroupEnum, IVertexProvider<TVertex>>> GetAllVertexProviders();
    }
}
