using Guppy.Core.Common.Providers;
using Guppy.Core.Common.Systems;
using Guppy.Core.Logging.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Systems;

namespace VoidHuntersRevived.Domain.Entities.Providers
{
    public class BelongsToSystemProvider(
        IEntityTemplateFragmentService entityTemplateFragmentService,
        IEntityQueryService entityQueryService,
        ILogger logger) : IScopedSystemProvider
    {
        private readonly IEntityTemplateFragmentService _entityTemplateFragmentService = entityTemplateFragmentService;
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly ILogger _logger = logger;

        public IEnumerable<IScopedSystem> GetSystems()
        {
            foreach (Type componentType in this._entityTemplateFragmentService.GetAllDistinctComponentTypes())
            {
                foreach (Type interfaceType in componentType.GetInterfaces())
                {
                    if (interfaceType.IsConstructedGenericType == false)
                    {
                        continue;
                    }

                    if (interfaceType.GetGenericTypeDefinition() == typeof(IBelongsTo<,>))
                    {
                        Type belongsToSystemType = typeof(BelongsToSystem<,>).MakeGenericType(interfaceType.GenericTypeArguments);
                        IScopedSystem belongsToSystem = (IScopedSystem?)Activator.CreateInstance(belongsToSystemType, [this._entityQueryService, this._logger]) ?? throw new NotImplementedException();

                        yield return belongsToSystem;
                    }

                    if (interfaceType.GetGenericTypeDefinition() == typeof(ICompositeBelongsTo<,,>))
                    {
                        Type belongsToSystemType = typeof(CompositeBelongsToEngine<,,>).MakeGenericType(interfaceType.GenericTypeArguments);
                        IScopedSystem belongsToSystem = (IScopedSystem?)Activator.CreateInstance(belongsToSystemType, [this._entityQueryService, this._logger]) ?? throw new NotImplementedException();

                        yield return belongsToSystem;
                    }
                }
            }
        }
    }
}