using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Domain.Common.Providers;
using VoidHuntersRevived.Domain.Graphics.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Providers;
using VoidHuntersRevived.Domain.Graphics.Common.Services;
using VoidHuntersRevived.Domain.Graphics.Providers;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Graphics.Services
{
    public class VertexTypeService : StrategyEngine, IVertexTypeService
    {
        private readonly IPrimitiveService _primitiveService;
        private readonly IUniqueNumberProvider _uniqueNumberProvider;
        private readonly Dictionary<Type, IVertexTypeManagerProvider> _vertexTypeManagerProvidersByVertexType;

        public VertexTypeService(
            IUniqueNumberProvider uniqueNumberProvider,
            IPrimitiveService primitiveService)
        {
            _uniqueNumberProvider = uniqueNumberProvider;
            _primitiveService = primitiveService;
            _vertexTypeManagerProvidersByVertexType = [];

            IReadOnlyDictionary<Type, IPrimitive[]> primitivesByVertexType = _primitiveService.GetAllByVertexType();
            foreach ((Type vertexType, IPrimitive[] primitives) in primitivesByVertexType)
            {
                Type vertexTypeManagerProviderType = typeof(VertexTypeManagerProvider<>).MakeGenericType(vertexType);
                IVertexTypeManagerProvider vertexTypeManagerProvider = (IVertexTypeManagerProvider)(Activator.CreateInstance(vertexTypeManagerProviderType, [_uniqueNumberProvider, primitives]) ?? throw new NotImplementedException());

                _vertexTypeManagerProvidersByVertexType.Add(vertexType, vertexTypeManagerProvider);
            }
        }

        public IReadOnlyDictionary<Type, IVertexTypeManagerProvider> GetAllByVertexType()
        {
            return _vertexTypeManagerProvidersByVertexType;
        }

        public IVertexTypeManagerProvider<TVertex> GetByVertexType<TVertex>() where TVertex : unmanaged, IVertexType
        {
            return (IVertexTypeManagerProvider<TVertex>)_vertexTypeManagerProvidersByVertexType[typeof(TVertex)];
        }
    }
}
