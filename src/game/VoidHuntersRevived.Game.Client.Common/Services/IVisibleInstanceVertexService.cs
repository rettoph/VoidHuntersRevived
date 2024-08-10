using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Game.Client.Common.Graphics.Vertices;

namespace VoidHuntersRevived.Game.Client.Common.Services
{
    public interface IVisibleInstanceVertexService : IInstanceVertexService<VertexInstanceVisible, IKey<IEntityType>>
    {
    }
}
