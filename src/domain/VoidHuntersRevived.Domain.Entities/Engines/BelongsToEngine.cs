using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Extensions.System;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Entities.Engines
{
    /// <summary>
    /// Magic engine that will handle adding and removing new entities with <see cref="BelongsTo{,}"/> components
    /// to their respective parent filters.
    /// 
    /// Instances of this engine are automatically created within the <see cref="Providers.BelongsToEngineProvider"/>
    /// </summary>
    /// <typeparam name="TOwner"></typeparam>
    /// <typeparam name="TItems"></typeparam>
    internal sealed class BelongsToEngine<TBelongsTo, TOwner>(IEntityQueryService entityQueryService, ILogger logger) : StrategyEngine, IOnSpawnEngine<TBelongsTo>
        where TBelongsTo : unmanaged, IBelongsTo<TOwner, TBelongsTo>
        where TOwner : unmanaged, IHasMany<TBelongsTo>
    {
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly ILogger _logger = logger;

        [SequenceGroup<OnSpawnSequenceGroupEnum>(OnSpawnSequenceGroupEnum.Group03)]
        public void OnSpawn(VhId sourceEventId, IEntityTemplate template, EntityId id, ref TBelongsTo belongsTo, in GroupIndex groupIndex)
        {
            if (belongsTo.OwnerFilterId == default)
            {
                _logger.Warning("{0}::{1} - Empty OwnerId", typeof(BelongsToEngine<TBelongsTo, TOwner>).GetFormattedName(), nameof(BelongsToEngine<TBelongsTo, TOwner>.OnSpawn));
                return;
            }

            _entityQueryService.GetFilter(belongsTo.OwnerFilterId).Add(id, groupIndex);
        }
    }
}
