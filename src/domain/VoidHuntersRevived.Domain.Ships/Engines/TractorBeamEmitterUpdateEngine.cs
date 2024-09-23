using Guppy.Core.Common.Attributes;
using Microsoft.Xna.Framework;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.FixedPoint.Extensions;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Ships.Engines
{
    [AutoLoad]
    [SequenceGroup<EngineSequence>(EngineSequence.Group03)]
    [SequenceGroup<StepSequence>(StepSequence.Step)]
    internal sealed class TractorBeamEmitterUpdateEngine : StrategyEngine,
        IStepEngine<Step>
    {
        private readonly IEntityQueryService _entityQueryService;
        private readonly ISpace _space;
        private readonly ILogger _logger;
        private readonly ITractorBeamEmitterService _tractorBeamEmitterService;
        private readonly ISocketService _socketService;

        public string name { get; } = nameof(TractorBeamEmitterUpdateEngine);

        public TractorBeamEmitterUpdateEngine(
            IEntityQueryService entityQueryService,
            ISpace space,
            ILogger logger,
            ITractorBeamEmitterService tractorBeamEmitterService,
            ISocketService socketService)
        {
            _entityQueryService = entityQueryService;
            _space = space;
            _logger = logger;
            _tractorBeamEmitterService = tractorBeamEmitterService;
            _socketService = socketService;
        }

        public void Step(in Step _param)
        {
            foreach (var ((vhids, tacticals, tractorBeamEmitters, count), _) in _entityQueryService.QueryEntities<EntityId, Tactical, TractorBeamEmitter>())
            {
                for (int i = 0; i < count; i++)
                {
                    this.UpdateTractorBeamEmitterTractorables(in vhids[i], ref tacticals[i], ref tractorBeamEmitters[i]);
                }
            }
        }

        private void UpdateTractorBeamEmitterTractorables(in EntityId tractorBeamEmitterId, ref Tactical tactical, ref TractorBeamEmitter tractorBeamEmitter)
        {
            ref var filter = ref _tractorBeamEmitterService.GetTractorableFilter(tractorBeamEmitterId);
            foreach (var (indices, groupId) in filter)
            {
                var (entityIds, statuses, enableds, _) = _entityQueryService.QueryEntities<EntityId, EntityStatus, Enabled>(groupId);

                for (int i = 0; i < indices.count; i++)
                {
                    uint index = indices[i];
                    ref EntityId tractorableId = ref entityIds[index];

                    if (statuses[index].IsDespawned == false)
                    {
                        if (enableds[index] == true)
                        {
                            IBody targetBody = _space.GetBody(in tractorableId);

                            EntityId targetId = _entityQueryService.GetId(tractorableId.VhId);
                            ref Tree target = ref _entityQueryService.QueryById<Tree>(targetId);

                            Location targetHeadChildLocation = _entityQueryService.QueryById<Plug>(target.HeadId).Location;

                            if (_socketService.TryGetClosestOpenSocket(tractorBeamEmitterId, tactical.Value, out var openSocketNode))
                            {
                                FixMatrix potentialTransformation = targetHeadChildLocation.Transformation.Invert() * openSocketNode.Transformation;
                                FixVector2 potentialPosition = FixVector2.Transform(FixVector2.Zero, potentialTransformation);

                                targetBody.SetTransform(potentialPosition, potentialTransformation.Radians());

                                return;
                            }

                            FixVector2 targetHeadChildNodePosition = FixVector2.Transform(FixVector2.Zero, targetHeadChildLocation.Transformation * FixMatrix.CreateRotationZ(targetBody.Rotation));
                            targetBody.SetTransform(tactical.Value - targetHeadChildNodePosition, targetBody.Rotation);
                        }
                        else
                        {
                            _logger.Warning("{ClassName}::{MethodName} - TractorBeamEmitter = {TractorBeamEmitterId}, Tractorable = {TractorableId}, Enabled = {Enabled}.", nameof(TractorBeamEmitterUpdateEngine), nameof(UpdateTractorBeamEmitterTractorables), tractorBeamEmitterId.VhId, tractorableId.VhId, enableds[index]);
                            _tractorBeamEmitterService.Deselect(
                                sourceId: HashBuilder<TractorBeamEmitterUpdateEngine, VhId>.Instance.Calculate(tractorableId.VhId),
                                tractorBeamEmitterId: tractorBeamEmitterId,
                                attachToSocketVhId: null);
                        }
                    }
                    else
                    {
                        _logger.Warning("{ClassName}::{MethodName} - TractorBeamEmitter = {TractorBeamEmitterId}, Tractorable = {TractorableId}, IsDespawned = {IsDespawned}.", nameof(TractorBeamEmitterUpdateEngine), nameof(UpdateTractorBeamEmitterTractorables), tractorBeamEmitterId.VhId, tractorableId.VhId, statuses[index].IsDespawned);
                    }
                }
            }
        }
    }
}
