using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Graphics.Common.Services
{
    public interface IPrimitiveService
    {
        IPrimitive GetByEntityTypeKey(Key<IEntityType> entityTypeKey);
        IPrimitive<TVertex> GetByEntityTypeKey<TVertex>(Key<IEntityType> entityTypeKey)
            where TVertex : unmanaged, IVertexType;

        IPrimitive GetByVertexType(Type vertexType);
        IPrimitive<TVertex> GetByVertexType<TVertex>()
            where TVertex : unmanaged, IVertexType;

        IEnumerable<IPrimitive> GetAllByVertexType(Type vertexType);
        IEnumerable<IPrimitive<TVertex>> GetAllByVertexType<TVertex>()
            where TVertex : unmanaged, IVertexType;

        IReadOnlyDictionary<Type, IPrimitive[]> GetAllByVertexType();

        IEnumerable<IPrimitive> GetAll();
    }
}
