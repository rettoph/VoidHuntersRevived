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
    [AutoLoad]
    internal sealed class TractorableEngine(
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
        public void OnSpawn(VhId sourceEventId, IEntityTemplate template, EntityId id, ref Tractorable tractorable, in GroupIndex groupIndex)
        {
            if (tractorable.TractorBeamEmitter == default)
            {
                return;
            }

            // Add the tractorable to its owning tractor beam emitter's filter
            ref var filter = ref _tractorBeamEmitterService.GetTractorableFilter(tractorable.TractorBeamEmitter);
            filter.Add(in id, in groupIndex);

            _tacticalService.AddUse(tractorable.TractorBeamEmitter);
            _logger.Verbose("{ClassName}::{MethodName} - Added tractorable {TractorableId} to emitter {TractorBeamEmitterId}", nameof(TractorableEngine), nameof(OnSpawn), id.VhId.Value, tractorable.TractorBeamEmitter.VhId.Value);
        }

        [SequenceGroup<OnDespawnSequenceGroupEnum>(OnDespawnSequenceGroupEnum.Group03)]
        public void OnDespawn(VhId sourceEventId, IEntityTemplate template, EntityId id, ref Tractorable tractorable, in GroupIndex groupIndex)
        {
            if (tractorable.TractorBeamEmitter == default)
            {
                return;
            }

            ref var filter = ref _tractorBeamEmitterService.GetTractorableFilter(tractorable.TractorBeamEmitter);
            filter.Remove(id);

            _tacticalService.RemoveUse(tractorable.TractorBeamEmitter);
            _logger.Verbose("{ClassName}::{MethodName} - Removed tractorable {TractorableId} from emitter {TractorBeamEmitterId}", nameof(TractorableEngine), nameof(OnDespawn), id, tractorable.TractorBeamEmitter.VhId.Value);
        }
    }
}
