using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Domain.Graphics.Common.Providers;

namespace VoidHuntersRevived.Domain.Graphics.Common.Services
{
    public interface IVertexTypeService
    {
        public IVertexTypeManagerProvider<TVertex> GetByVertexType<TVertex>()
            where TVertex : unmanaged, IVertexType;

        public IReadOnlyDictionary<Type, IVertexTypeManagerProvider> GetAllByVertexType();
    }
}
