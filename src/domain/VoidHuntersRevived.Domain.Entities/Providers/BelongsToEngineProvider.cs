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
