using Guppy.Core.Common.Attributes;
using Guppy.Core.Logging.Common;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Extensions.Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;

namespace VoidHuntersRevived.Domain.Pieces.Systems
{
    public sealed class TractorableSystem(
        ITacticalService tacticalService,
        IEntityQueryService entityQueryService,
        ILogger logger) : StrategySystem,
        IOnSpawnEngine<Tractorable>,
        IOnDespawnEngine<Tractorable>
    {
        private readonly ITacticalService _tacticalService = tacticalService;
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly ILogger _logger = logger;

        [SequenceGroup<OnSpawnSequenceGroupEnum>(OnSpawnSequenceGroupEnum.Group03)]
        public void OnSpawn(VhId sourceEventId, IEntityTemplate template, ref Entity<Tractorable> tractorable)
        {
            if (tractorable.Component.TractorBeamEmitterFilterId.EGID == default)
            {
                return;
            }

            // Add the tractorable to its owning tractor beam emitter's filter
            ref var filter = ref this._entityQueryService.GetFilter(tractorable.Component.TractorBeamEmitterFilterId);
            filter.Add(in tractorable.LocalId, in tractorable.Index);

            this._tacticalService.AddUse(tractorable.Component.TractorBeamEmitterFilterId.EGID.ToEntityLocalId());
            this._logger.Verbose("Added tractorable {TractorableId} to emitter {TractorBeamEmitterId}", tractorable.LocalId, tractorable.Component.TractorBeamEmitterFilterId);
        }

        [SequenceGroup<OnDespawnSequenceGroupEnum>(OnDespawnSequenceGroupEnum.Group03)]
        public void OnDespawn(VhId sourceEventId, IEntityTemplate template, ref Entity<Tractorable> tractorable)
        {
            if (tractorable.Component.TractorBeamEmitterFilterId.EGID == default)
            {
                return;
            }

            ref var filter = ref this._entityQueryService.GetFilter(tractorable.Component.TractorBeamEmitterFilterId);
            filter.Remove(tractorable.LocalId);

            this._tacticalService.RemoveUse(tractorable.Component.TractorBeamEmitterFilterId.EGID.ToEntityLocalId());
            this._logger.Verbose("Removed tractorable {TractorableId} from emitter {TractorBeamEmitterId}", tractorable.LocalId, tractorable.Component.TractorBeamEmitterFilterId);
        }
    }
}