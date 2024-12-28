using Guppy.Core.Common.Attributes;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Ships.Engines
{
    public sealed class TractorBeamEmitterUpdateEngine(
        IEntityQueryService entityQueryService,
        ISpace space,
        ILogger logger,
        ITractorBeamEmitterService tractorBeamEmitterService,
        INodeSocketService socketService) : StrategyEngine,
        IOnStepEngine
    {
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly ISpace _space = space;
        private readonly ILogger _logger = logger;
        private readonly ITractorBeamEmitterService _tractorBeamEmitterService = tractorBeamEmitterService;
        private readonly INodeSocketService _socketService = socketService;

        [SequenceGroup<OnStepSequenceGroup>(OnStepSequenceGroup.ProcessInput)]
        public void OnStep(Step step)
        {
            foreach (var ((localIds, tacticals, tractorBeamEmitters, count), _) in _entityQueryService.QueryEntities<EntityLocalId, Tactical, TractorBeamEmitter>())
            {
                for (int i = 0; i < count; i++)
                {
                    this.UpdateTractorBeamEmitterTractorables(in localIds[i], ref tacticals[i], ref tractorBeamEmitters[i]);
                }
            }
        }

        private void UpdateTractorBeamEmitterTractorables(in EntityLocalId tractorBeamEmitterLocalId, ref Tactical tactical, ref TractorBeamEmitter tractorBeamEmitter)
        {
            ref var filter = ref _tractorBeamEmitterService.GetTractorableFilter(tractorBeamEmitterLocalId);
            foreach (var (indices, groupId) in filter)
            {
                var (localIds, statuses, enableds, trees, _) = _entityQueryService.QueryEntities<EntityLocalId, EntityStatus, Enabled, Tree>(groupId);

                for (int i = 0; i < indices.count; i++)
                {
                    uint index = indices[i];
                    ref EntityLocalId targetId = ref localIds[index];

                    if (statuses[index].IsDespawned == true)
                    {
                        _logger.Warning("Despanwed - TractorBeamEmitter = {TractorBeamEmitterId}, TractorBeamEmitterLocalId = {TractorBeamEmitterLocalId}, IsDespawned = {IsDespawned}.", tractorBeamEmitterLocalId, targetId, statuses[index].IsDespawned);
                        continue;
                    }

                    if (enableds[index] == false)
                    {
                        _logger.Warning("Not Enabled - TractorBeamEmitter = {TractorBeamEmitterId}, TractorBeamEmitterLocalId = {TractorBeamEmitterLocalId}, Enabled = {Enabled}.", tractorBeamEmitterLocalId, targetId, enableds[index].Value);
                        throw new NotImplementedException();

                        // What to do here?
                        // We want to deselect the current item in the tractorbeam - but how to generate a sourceId?
                        // Its possible a sourceId is required here since this method call can 'spawn' its own events
                        // _tractorBeamEmitterService.Deselect(
                        //     sourceId: HashBuilder<TractorBeamEmitterUpdateEngine, VhId>.Instance.Calculate(targetId.VhId),
                        //     tractorBeamEmitterId: tractorBeamEmitterId,
                        //     attachToSocketVhId: null);
                        // 
                        // continue;
                    }

                    IBody targetBody = _space.GetBody(in targetId);
                    ref Tree targetTree = ref trees[index];

                    FixTransform2D targetHeadChildTransform = _entityQueryService.QueryByLocalId<Plug>(targetTree.HeadLocalId).NodeTransform;

                    if (_socketService.TryGetClosestOpenNodeSocket(tractorBeamEmitterLocalId, tactical.Value, out var openSocketNode))
                    {
                        FixTransform2D potentialTransform = FixTransform2D.Invert(targetHeadChildTransform) * openSocketNode.WorldTransform;

                        targetBody.SetTransform(potentialTransform);

                        return;
                    }

                    FixVector2 targetHeadChildNodePosition = FixVector2.Transform(FixVector2.Zero, targetHeadChildTransform * FixTransform2D.CreateRotation(targetBody.Rotation));
                    FixTransform2D transform = new(targetBody.Transform.Rotation, tactical.Value - targetHeadChildNodePosition);
                    targetBody.SetTransform(transform);
                }
            }
        }
    }
}
