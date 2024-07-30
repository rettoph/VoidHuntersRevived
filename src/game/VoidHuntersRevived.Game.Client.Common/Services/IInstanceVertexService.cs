using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Game.Client.Common.Utilities;

namespace VoidHuntersRevived.Game.Client.Common.Services
{
    public interface IInstanceVertexService
    {
        void Initialize();
    }

    public interface IInstanceVertexService<TVertex, TId> : IInstanceVertexService
        where TVertex : struct, IVertexType
        where TId : struct
    {
        InstanceVertexProvider<TVertex> GetInstanceVertexProviderById(TId id);

        IEnumerable<InstanceVertexProvider<TVertex>> GetAllInstanceVertexProviders();
    }
}
