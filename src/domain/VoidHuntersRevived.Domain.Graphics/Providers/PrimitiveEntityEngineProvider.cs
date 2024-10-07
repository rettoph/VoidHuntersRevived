using Autofac;
using Guppy.Core.Common;
using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Graphics.Common.Services;
using VoidHuntersRevived.Domain.Graphics.Engines;

namespace VoidHuntersRevived.Domain.Graphics.Providers
{
    [AutoLoad]
    public class PrimitiveEntityEngineProvider(
        IPrimitiveService primitiveService,
        IEntityTypeService entityTypeService,
        IEntityQueryService entityQueryService,
        ILifetimeScope scope) : IEngineProvider
    {
        private readonly IPrimitiveService _primitiveService = primitiveService;
        private readonly IEntityTypeService _entityTypeService = entityTypeService;
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly ILifetimeScope _scope = scope;

        public IEnumerable<IEngine> GetEngines()
        {
            foreach (Type vertexType in _primitiveService.GetAllVertexTypes())
            {
                if (vertexType.IsAssignableTo<IEntityComponent>() == false)
                {
                    continue;
                }

                Type vertexTypePrimitiveServiceType = typeof(IPrimitiveService<>).MakeGenericType(vertexType);
                object vertexTypePrimitiveService = _scope.Resolve(vertexTypePrimitiveServiceType);
                yield return PrimitiveEntityEngineProvider.BuildEngine(vertexType, vertexTypePrimitiveService, _entityQueryService);
            }
        }

        private static IEngine BuildEngine(Type vertexType, object vertexTypePrimitiveService, IEntityQueryService entityQueryService)
        {
            Type engineType = typeof(PrimitiveEntityEngine<>).MakeGenericType(vertexType);
            ThrowIf.Type.IsNotAssignableFrom<IEngine>(engineType);

            IEngine genericEngine = (IEngine)(Activator.CreateInstance(engineType, [vertexTypePrimitiveService, entityQueryService]) ?? throw new NotImplementedException());

            return genericEngine;
        }
    }
}
