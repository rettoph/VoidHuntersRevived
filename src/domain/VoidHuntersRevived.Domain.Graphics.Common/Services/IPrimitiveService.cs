using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Graphics.Common.Services
{
    public interface IPrimitiveService
    {
        IPrimitive GetPrimitiveByEntityTypeKey(IKey<IEntityType> entityTypeKey);
        IPrimitive<TVertex> GetPrimitiveByEntityTypeKey<TVertex>(IKey<IEntityType> entityTypeKey)
            where TVertex : unmanaged, IVertexType;
    }
}
