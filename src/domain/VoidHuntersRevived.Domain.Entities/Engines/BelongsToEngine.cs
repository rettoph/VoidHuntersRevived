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
    /// <typeparam name="TBelongsTo"></typeparam>
    /// <typeparam name="TParent"></typeparam>
    internal sealed class BelongsToEngine<TParent, TBelongsTo>(IEntityQueryService entityQueryService, ILogger logger) : StrategyEngine, IOnSpawnEngine<TBelongsTo>
        where TParent : unmanaged, IHasMany<TBelongsTo>
        where TBelongsTo : unmanaged, IBelongsTo<TParent, TBelongsTo>
    {
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly ILogger _logger = logger;

        [SequenceGroup<OnSpawnSequenceGroupEnum>(OnSpawnSequenceGroupEnum.Group02)]
        public void OnSpawn(VhId sourceEventId, IEntityTemplate template, ref Entity<TBelongsTo> belongsTo)
        {
            if (belongsTo.Component.ParentFilterId.IsDefault<TParent>())
            {
                _logger.Warning("{0}::{1} - Empty {2}", typeof(BelongsToEngine<TParent, TBelongsTo>).GetFormattedName(), nameof(BelongsToEngine<TParent, TBelongsTo>.OnSpawn), nameof(belongsTo.Component.ParentFilterId));
                return;
            }

            _entityQueryService.GetFilter(belongsTo.Component.ParentFilterId).Add(in belongsTo.LocalId, in belongsTo.Index);
        }
    }
}
