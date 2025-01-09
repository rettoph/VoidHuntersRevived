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
    /// <typeparam name="TSecondary"></typeparam>
    /// <typeparam name="TParent"></typeparam>
    internal sealed class CompositeBelongsToEngine<TParent, TPrimary, TSecondary>(IEntityQueryService entityQueryService, ILogger logger) : StrategyEngine, IOnSpawnEngine<TPrimary, TSecondary>
        where TParent : unmanaged, IHasMany<TPrimary>
        where TPrimary : unmanaged, IBelongsTo<TParent, TPrimary>
        where TSecondary : unmanaged, ICompositeBelongsTo<TParent, TPrimary, TSecondary>
    {
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly ILogger _logger = logger;

        [SequenceGroup<OnSpawnSequenceGroupEnum>(OnSpawnSequenceGroupEnum.Group02)]
        public void OnSpawn(VhId sourceEventId, IEntityTemplate template, ref Entity<TPrimary, TSecondary> entity)
        {
            if (entity.First.ParentFilterId.IsDefault<TParent>())
            {
                this._logger.Warning("{0}::{1} - Empty {2}", typeof(CompositeBelongsToEngine<TParent, TPrimary, TSecondary>).GetFormattedName(), nameof(CompositeBelongsToEngine<TParent, TPrimary, TSecondary>.OnSpawn), nameof(entity.First.ParentFilterId));
                return;
            }

            this._entityQueryService.GetCompositeFilter<TParent, TPrimary, TSecondary>(entity.First).Add(in entity.LocalId, in entity.Index);
        }
    }
}