using Guppy.Core.Common.Attributes;
using Guppy.Core.Logging.Common;
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
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Ships.Engines
{
    public sealed class TractorBeamEmitterUpdateEngine(
        IEntityQueryService entityQueryService,
        ISpace space,
        ILogger logger,
        INodeSocketService socketService) : StrategyEngine,
        IOnStepEngine
    {
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly ISpace _space = space;
        private readonly ILogger _logger = logger;
        private readonly INodeSocketService _socketService = socketService;

        [SequenceGroup<OnStepSequenceGroupEnum>(OnStepSequenceGroupEnum.ProcessInput)]
        public void OnStep(Step step)
        {
            foreach (var ((localIds, tacticals, tractorBeamEmitters, count), _) in this._entityQueryService.QueryEntities<EntityLocalId, Tactical, TractorBeamEmitter>())
            {
                for (int i = 0; i < count; i++)
                {
                    this.UpdateTractorBeamEmitterTractorables(in localIds[i], ref tacticals[i], ref tractorBeamEmitters[i]);
                }
            }
        }

        private void UpdateTractorBeamEmitterTractorables(in EntityLocalId tractorBeamEmitterLocalId, ref Tactical tactical, ref TractorBeamEmitter tractorBeamEmitter)
        {
            ref var filter = ref this._entityQueryService.GetFilter(tractorBeamEmitter.TractorableFilterId);
            foreach (var (indices, groupId) in filter)
            {
                var (localIds, statuses, enableds, trees, _) = this._entityQueryService.QueryEntities<EntityLocalId, EntityStatus, Enabled, Tree>(groupId);

                for (int i = 0; i < indices.count; i++)
                {
                    uint index = indices[i];
                    ref EntityLocalId targetId = ref localIds[index];

                    if (statuses[index].IsDespawned == true)
                    {
                        this._logger.Warning("Despawned. TractorBeamEmitter = {TractorBeamEmitterId}, TractorBeamEmitterLocalId = {TractorBeamEmitterLocalId}, IsDespawned = {IsDespawned}.", tractorBeamEmitterLocalId, targetId, statuses[index].IsDespawned);
                        continue;
                    }

                    if (enableds[index] == false)
                    {
                        this._logger.Warning("Not Enabled. TractorBeamEmitter = {TractorBeamEmitterId}, TractorBeamEmitterLocalId = {TractorBeamEmitterLocalId}, Enabled = {Enabled}.", tractorBeamEmitterLocalId, targetId, enableds[index].Value);
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

                    IBody targetBody = this._space.GetBody(in targetId);
                    ref Tree targetTree = ref trees[index];

                    FixTransform2D targetHeadChildTransform = this._entityQueryService.QueryByLocalId<Plug>(targetTree.HeadLocalId).NodeTransform;

                    if (this._socketService.TryGetClosestOpenNodeSocket(tractorBeamEmitterLocalId, tactical.Value, out var openSocketNode))
                    {
                        FixTransform2D potentialTransform = FixTransform2D.Invert(targetHeadChildTransform) * openSocketNode.WorldTransform;

                        targetBody.SetTransform(potentialTransform);

                        return;
                    }

                    FixVector2 targetHeadChildNodePosition = FixVector2.Transform(FixVector2.Zero, targetHeadChildTransform * FixTransform2D.CreateRotation(targetBody.Rotation));
                    targetBody.Position = tactical.Value - targetHeadChildNodePosition;
                }
            }
        }
    }
}