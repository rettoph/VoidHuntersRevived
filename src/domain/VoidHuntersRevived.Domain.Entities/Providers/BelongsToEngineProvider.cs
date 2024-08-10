using Guppy.Core.Common.Attributes;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Engines;

namespace VoidHuntersRevived.Domain.Entities.Providers
{
    [AutoLoad]
    internal class BelongsToEngineProvider : IEngineProvider
    {
        private readonly IEntityTypeProviderService _entityTypeService;
        private readonly IEntityQueryService _entityQueryService;
        private readonly ILogger _logger;

        public BelongsToEngineProvider(
            IEntityTypeProviderService entityTypeService,
            IEntityQueryService entityQueryService,
            ILogger logger)
        {
            _entityTypeService = entityTypeService;
            _entityQueryService = entityQueryService;
            _logger = logger;
        }

        public IEnumerable<IEngine> GetEngines()
        {
            foreach (Type componentType in _entityTypeService.GetAllDistinctComponentTypes())
            {
                if (componentType.IsConstructedGenericType == false)
                {
                    continue;
                }

                if (componentType.GetGenericTypeDefinition() != typeof(BelongsTo<,>))
                {
                    continue;
                }


                Type belongsToEngineType = typeof(BelongsToEngine<,>).MakeGenericType(componentType.GenericTypeArguments);
                IEngine belongsToEngine = (IEngine?)Activator.CreateInstance(belongsToEngineType, new object[] { _entityQueryService, _logger }) ?? throw new NotImplementedException();

                yield return belongsToEngine;
            }
        }
    }
}
