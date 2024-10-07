using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;

namespace VoidHuntersRevived.Domain.Graphics.Common.Services
{
    public interface IPrimitiveService
    {
        IEnumerable<Type> GetAllVertexTypes();

        IEnumerable<IPrimitive> GetAll();
        IEnumerable<IPrimitive<TVertex>> GetAll<TVertex>()
        where TVertex : unmanaged, IVertexType;
    }

    public interface IPrimitiveService<TVertex>
        where TVertex : unmanaged, IVertexType
    {
        IPrimitive<TVertex>[] GetAll();
        IPrimitive<TVertex> GetPrimitiveByTypeAndSequenceGroup(Key<IPrimitiveType> type, PrimitiveSequenceGroupEnum sequenceGroup);
    }
}
