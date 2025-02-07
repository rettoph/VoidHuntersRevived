using Autofac;
using Guppy.Core.Common;
using Guppy.Core.Common.Providers;
using Guppy.Core.Common.Systems;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Graphics.Common.Services;
using VoidHuntersRevived.Domain.Graphics.Systems;

namespace VoidHuntersRevived.Domain.Graphics.Providers
{
    public class PrimitiveEntitySystemProvider(
        IPrimitiveService primitiveService,
        IEntityQueryService entityQueryService,
        ILifetimeScope scope,
        EntitiesDB entitiesDb) : IScopedSystemProvider
    {
        private readonly IPrimitiveService _primitiveService = primitiveService;
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly ILifetimeScope _scope = scope;
        private readonly EntitiesDB _entitiesDb = entitiesDb;

        public IEnumerable<IScopedSystem> GetSystems()
        {
            foreach (Type vertexType in this._primitiveService.GetAllVertexTypes())
            {
                if (vertexType.IsAssignableTo<IEntityComponent>() == false)
                {
                    continue;
                }

                Type vertexTypePrimitiveServiceType = typeof(IPrimitiveService<>).MakeGenericType(vertexType);
                object vertexTypePrimitiveService = this._scope.Resolve(vertexTypePrimitiveServiceType);
                yield return PrimitiveEntitySystemProvider.BuildSystem(vertexType, vertexTypePrimitiveService, this._entityQueryService, this._entitiesDb);
            }
        }

        private static IScopedSystem BuildSystem(Type vertexType, object vertexTypePrimitiveService, IEntityQueryService entityQueryService, EntitiesDB entitiesDb)
        {
            Type engineType = typeof(PrimitiveEntitySystem<>).MakeGenericType(vertexType);
            ThrowIf.Type.IsNotAssignableFrom<IEngine>(engineType);

            IScopedSystem genericEngine = (IScopedSystem)(Activator.CreateInstance(engineType, [vertexTypePrimitiveService, entityQueryService, entitiesDb]) ?? throw new NotImplementedException());

            return genericEngine;
        }
    }
}