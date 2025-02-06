using Guppy.Core.Common.Providers;
using Guppy.Core.Common.Systems;
using Guppy.Core.Logging.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Systems;

namespace VoidHuntersRevived.Domain.Entities.Providers
{
    public class BelongsToSystemProvider(
        IEntityTemplateFragmentService entityTemplateService,
        IEntityQueryService entityQueryService,
        ILogger logger) : IScopedSystemProvider
    {
        private readonly IEntityTemplateFragmentService _entityTemplateService = entityTemplateService;
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly ILogger _logger = logger;

        public IEnumerable<IScopedSystem> GetSystems()
        {
            foreach (Type componentType in this._entityTemplateService.GetAllDistinctComponentTypes())
            {
                foreach (Type interfaceType in componentType.GetInterfaces())
                {
                    if (interfaceType.IsConstructedGenericType == false)
                    {
                        continue;
                    }

                    if (interfaceType.GetGenericTypeDefinition() == typeof(IBelongsTo<,>))
                    {
                        Type belongsToEngineType = typeof(BelongsToSystem<,>).MakeGenericType(interfaceType.GenericTypeArguments);
                        IScopedSystem belongsToEngine = (IScopedSystem?)Activator.CreateInstance(belongsToEngineType, [this._entityQueryService, this._logger]) ?? throw new NotImplementedException();

                        yield return belongsToEngine;
                    }

                    if (interfaceType.GetGenericTypeDefinition() == typeof(ICompositeBelongsTo<,,>))
                    {
                        Type belongsToEngineType = typeof(CompositeBelongsToEngine<,,>).MakeGenericType(interfaceType.GenericTypeArguments);
                        IScopedSystem belongsToEngine = (IScopedSystem?)Activator.CreateInstance(belongsToEngineType, [this._entityQueryService, this._logger]) ?? throw new NotImplementedException();

                        yield return belongsToEngine;
                    }
                }
            }
        }
    }
}