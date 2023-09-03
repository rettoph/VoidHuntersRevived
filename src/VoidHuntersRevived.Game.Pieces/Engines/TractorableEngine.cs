﻿using Guppy.Attributes;
using Serilog;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Ships.Components;
using VoidHuntersRevived.Common.Ships.Services;
using VoidHuntersRevived.Common.Simulations.Engines;

namespace VoidHuntersRevived.Game.Pieces.Engines
{
    [AutoLoad]
    internal sealed class TractorableEngine : BasicEngine, IReactOnAddEx<Tractorable>, IReactOnRemoveEx<Tractorable>
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

        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<Tractorable> entities, ExclusiveGroupStruct groupID)
        {
            var (tractorables, ids, _) = entities;

            for (uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                ref Tractorable tractorable = ref tractorables[index];

                if (tractorable.TractorBeamEmitter == default)
                {
                    continue;
                }

                // Add the tractorable to its owning tractor beam emitter's filter
                ref var filter = ref _tractorBeamEmitters.GetTractorableFilter(tractorable.TractorBeamEmitter);
                filter.Add(ids[index], groupID, index);

                _tacticals.AddUse(tractorable.TractorBeamEmitter);

                EntityId tractorableId = _entities.QueryByGroupIndex<EntityId>(groupID, index);
                _logger.Verbose("{ClassName}::{MethodName} - Added tractorable {TractorableId} to emitter {TractorBeamEmitterId}", nameof(TractorableEngine), nameof(Add), tractorableId.VhId.Value, tractorable.TractorBeamEmitter.VhId.Value);
            }
        }

        public void Remove((uint start, uint end) rangeOfEntities, in EntityCollection<Tractorable> entities, ExclusiveGroupStruct groupID)
        {
            var (tractorables, ids, _) = entities;

            for (uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                ref Tractorable tractorable = ref tractorables[index];

                if (tractorable.TractorBeamEmitter == default)
                {
                    continue;
                }

                ref var filter = ref _tractorBeamEmitters.GetTractorableFilter(tractorable.TractorBeamEmitter);
                filter.Remove(ids[index], groupID);

                _tacticals.RemoveUse(tractorable.TractorBeamEmitter);

                EntityId tractorableId = _entities.QueryByGroupIndex<EntityId>(groupID, index);
                _logger.Verbose("{ClassName}::{MethodName} - Removed tractorable {TractorableId} from emitter {TractorBeamEmitterId}", nameof(TractorableEngine), nameof(Remove), tractorableId.VhId.Value, tractorable.TractorBeamEmitter.VhId.Value);
            }
        }
    }
}
