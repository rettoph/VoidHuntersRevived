using Guppy.Core.Common.Attributes;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Pieces.Engines
{
    public sealed class TractorableEngine(
        ITractorBeamEmitterService tractorBeamEmitterService,
        ITacticalService tacticalService,
        IEntityQueryService entityQueryService,
        ILogger logger) : StrategyEngine,
        IOnSpawnEngine<Tractorable>,
        IOnDespawnEngine<Tractorable>
    {
        private readonly ITractorBeamEmitterService _tractorBeamEmitterService = tractorBeamEmitterService;
        private readonly ITacticalService _tacticalService = tacticalService;
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly ILogger _logger = logger;

        [SequenceGroup<OnSpawnSequenceGroupEnum>(OnSpawnSequenceGroupEnum.Group03)]
        public void OnSpawn(VhId sourceEventId, IEntityTemplate template, ref Entity<Tractorable> tractorable)
        {
            if (tractorable.Component.TractorBeamEmitterLocalId == default)
            {
                return;
            }

            // Add the tractorable to its owning tractor beam emitter's filter
            ref var filter = ref _tractorBeamEmitterService.GetTractorableFilter(tractorable.Component.TractorBeamEmitterLocalId);
            filter.Add(in tractorable.LocalId, in tractorable.Index);

            _tacticalService.AddUse(tractorable.Component.TractorBeamEmitterLocalId);
            _logger.Verbose("Added tractorable {TractorableId} to emitter {TractorBeamEmitterId}", tractorable.LocalId, tractorable.Component.TractorBeamEmitterLocalId);
        }

        [SequenceGroup<OnDespawnSequenceGroupEnum>(OnDespawnSequenceGroupEnum.Group03)]
        public void OnDespawn(VhId sourceEventId, IEntityTemplate template, ref Entity<Tractorable> tractorable)
        {
            if (tractorable.Component.TractorBeamEmitterLocalId == default)
            {
                return;
            }

            ref var filter = ref _tractorBeamEmitterService.GetTractorableFilter(tractorable.Component.TractorBeamEmitterLocalId);
            filter.Remove(tractorable.LocalId);

            _tacticalService.RemoveUse(tractorable.Component.TractorBeamEmitterLocalId);
            _logger.Verbose("Removed tractorable {TractorableId} from emitter {TractorBeamEmitterId}", tractorable.LocalId, tractorable.Component.TractorBeamEmitterLocalId);
        }
    }
}
