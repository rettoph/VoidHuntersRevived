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
        private readonly IEntityTypeService _entityTypes;
        private readonly IEntityService _entities;
        private readonly ILogger _logger;

        public BelongsToEngineProvider(IEntityTypeService entityTypes, IEntityService entities, ILogger logger)
        {
            _entityTypes = entityTypes;
            _entities = entities;
            _logger = logger;
        }

        public IEnumerable<IEngine> GetEngines()
        {
            foreach (Type componentType in _entityTypes.GetAllDistinctComponentTypes())
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
                IEngine belongsToEngine = (IEngine?)Activator.CreateInstance(belongsToEngineType, new object[] { _entities, _logger }) ?? throw new NotImplementedException();

                yield return belongsToEngine;
            }
        }
    }
}
