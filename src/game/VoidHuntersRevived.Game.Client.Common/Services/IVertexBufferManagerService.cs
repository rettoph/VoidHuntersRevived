using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Game.Client.Common.Graphics.Vertices;
using VoidHuntersRevived.Game.Client.Common.Utilities;

namespace VoidHuntersRevived.Game.Client.Common.Services
{
    public interface IVertexBufferManagerService<TVertex, TId>
        where TVertex : struct, IVertexType
        where TId : struct
    {
        VertexBufferManager<VertexInstanceVisible> GetById(Id<IEntityType> id);
    }
}
