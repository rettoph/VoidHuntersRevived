using Autofac;
using Guppy.Core.Common.Providers;
using Guppy.Core.Common.Systems;
using Guppy.Core.Logging.Common;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Systems;

namespace VoidHuntersRevived.Domain.Entities.Providers
{
    public class DisposableSystemProvider(
        IEntityTemplateFragmentService entityTemplateFragmentService,
        EntitiesDB entitiesDb,
        ILogger logger) : IScopedSystemProvider
    {
        private readonly IEntityTemplateFragmentService _entityTemplateFragmentService = entityTemplateFragmentService;
        private readonly EntitiesDB _entitiesDb = entitiesDb;
        private readonly ILogger _logger = logger;

        public IEnumerable<IScopedSystem> GetSystems()
        {
            foreach (Type componentType in this._entityTemplateFragmentService.GetAllDistinctComponentTypes())
            {
                if (componentType.IsAssignableTo<IDisposable>() == false)
                {
                    continue;
                }

                Type disposableSystemType = typeof(DisposableSystem<>).MakeGenericType(componentType);
                IScopedSystem disposableSystem = (IScopedSystem?)Activator.CreateInstance(disposableSystemType, [this._entitiesDb, this._logger]) ?? throw new NotImplementedException();

                yield return disposableSystem;
            }
        }
    }
}
