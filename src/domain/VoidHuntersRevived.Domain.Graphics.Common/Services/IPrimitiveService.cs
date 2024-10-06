using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;

namespace VoidHuntersRevived.Domain.Graphics.Common.Services
{
    public interface IPrimitiveService
    {
        IEnumerable<Type> GetAllVertexTypes();

        IEnumerable<Primitive> GetAll();
        IEnumerable<Primitive<TVertex>> GetAll<TVertex>()
        where TVertex : unmanaged, IVertexType;
    }

    public interface IPrimitiveService<TVertex>
        where TVertex : unmanaged, IVertexType
    {
        Primitive<TVertex> GetPrimitiveByTypeAndSequenceGroup(Key<PrimitiveType> type, PrimitiveSequenceGroupEnum sequenceGroup);
    }
}
