using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;
using VoidHuntersRevived.Domain.Graphics.Common.Utilities;

namespace VoidHuntersRevived.Domain.Graphics.Common
{
    public interface IPrimitive
    {
        IKey<IEntityType>? EntityTypeKey { get; }
        Type VertexType { get; }
        PrimitiveGroupEnum[] Groups { get; }
        EntitiesDB EntitiesDb { set; }
    }

    public interface IPrimitive<TVertex> : IPrimitive
        where TVertex : unmanaged, IVertexType
    {
        IVertexBuffer<TVertex> GetVertexBuffer(PrimitiveGroupEnum primitiveGroup);
        IEnumerable<KeyValuePair<PrimitiveGroupEnum, IVertexBuffer<TVertex>>> GetAllVertexBuffers();
    }
}
