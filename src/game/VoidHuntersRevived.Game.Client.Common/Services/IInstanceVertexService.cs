using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Game.Client.Common.Utilities;

namespace VoidHuntersRevived.Game.Client.Common.Services
{
    public interface IInstanceVertexService
    {
        void Initialize();
    }

    public interface IInstanceVertexService<TVertex, TKey> : IInstanceVertexService
        where TVertex : struct, IVertexType
    {
        InstanceVertexProvider<TVertex> GetInstanceVertexProviderByKey(TKey key);

        IEnumerable<InstanceVertexProvider<TVertex>> GetAllInstanceVertexProviders();
    }
}
