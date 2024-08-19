using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Graphics.Vertices;

namespace VoidHuntersRevived.Game.Client.Common.Services
{
    public interface IVisibleInstanceVertexService : IInstanceVertexService<VertexInstanceVisible, IKey<IEntityType>>
    {
    }
}
