using Guppy.Attributes;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Pieces.Engines
{
    [AutoLoad]
    internal sealed class TractorableEngine : BasicEngine,
        IOnSpawnEngine<Tractorable>,
        IOnDespawnEngine<Tractorable>
    {
        private readonly ITractorBeamEmitterService _tractorBeamEmitters;
        private readonly ITacticalService _tacticals;
        private readonly IEntityService _entities;
        private readonly ILogger _logger;

        public TractorableEngine(ITractorBeamEmitterService tractorBeamEmitters, ITacticalService tacticals, IEntityService entities, ILogger logger)
        {
            _tractorBeamEmitters = tractorBeamEmitters;
            _tacticals = tacticals;
            _entities = entities;
            _logger = logger;
        }

        public void OnSpawn(VhId sourceEventId, EntityId id, ref Tractorable tractorable, in GroupIndex groupIndex)
        {
            if (tractorable.TractorBeamEmitter == default)
            {
                return;
            }

            // Add the tractorable to its owning tractor beam emitter's filter
            ref var filter = ref _tractorBeamEmitters.GetTractorableFilter(tractorable.TractorBeamEmitter);
            filter.Add(in id, in groupIndex);

            _tacticals.AddUse(tractorable.TractorBeamEmitter);
            _logger.Verbose("{ClassName}::{MethodName} - Added tractorable {TractorableId} to emitter {TractorBeamEmitterId}", nameof(TractorableEngine), nameof(OnSpawn), id.VhId.Value, tractorable.TractorBeamEmitter.VhId.Value);
        }

        public void OnDespawn(VhId sourceEventId, EntityId id, ref Tractorable tractorable, in GroupIndex groupIndex)
        {
            if (tractorable.TractorBeamEmitter == default)
            {
                return;
            }

            ref var filter = ref _tractorBeamEmitters.GetTractorableFilter(tractorable.TractorBeamEmitter);
            filter.Remove(id);

            _tacticals.RemoveUse(tractorable.TractorBeamEmitter);
            _logger.Verbose("{ClassName}::{MethodName} - Removed tractorable {TractorableId} from emitter {TractorBeamEmitterId}", nameof(TractorableEngine), nameof(OnDespawn), id, tractorable.TractorBeamEmitter.VhId.Value);
        }
    }
}
