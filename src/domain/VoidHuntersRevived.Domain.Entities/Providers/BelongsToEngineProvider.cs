using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Engines;

namespace VoidHuntersRevived.Domain.Entities.Providers
{
    internal class BelongsToEngineProvider(
        IEntityTemplateFragmentService entityTemplateService,
        IEntityQueryService entityQueryService,
        ILogger logger) : IEngineProvider
    {
        private readonly IEntityTemplateFragmentService _entityTemplateService = entityTemplateService;
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly ILogger _logger = logger;

        public IEnumerable<IEngine> GetEngines()
        {
            foreach (Type componentType in _entityTemplateService.GetAllDistinctComponentTypes())
            {
                foreach (Type interfaceType in componentType.GetInterfaces())
                {
                    if (interfaceType.IsConstructedGenericType == false)
                    {
                        continue;
                    }

                    if (interfaceType.GetGenericTypeDefinition() != typeof(IBelongsTo<,>))
                    {
                        continue;
                    }

                    Type belongsToEngineType = typeof(BelongsToEngine<,>).MakeGenericType(componentType, interfaceType.GenericTypeArguments[0]);
                    IEngine belongsToEngine = (IEngine?)Activator.CreateInstance(belongsToEngineType, new object[] { _entityQueryService, _logger }) ?? throw new NotImplementedException();

                    yield return belongsToEngine;
                }
            }
        }
    }
}
