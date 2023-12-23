using Guppy.Attributes;
using Microsoft.Xna.Framework;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.FixedPoint.Extensions;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Common.Ships.Components;
using VoidHuntersRevived.Common.Ships.Services;
using VoidHuntersRevived.Common.Simulations.Engines;

namespace VoidHuntersRevived.Game.Ships.Engines
{
    [AutoLoad]
    internal sealed class TractorBeamEmitterUpdateEngine : BasicEngine,
        IStepEngine<Step>
    {
        private readonly IEntityService _entities;
        private readonly ISpace _space;
        private readonly ILogger _logger;
        private readonly ITractorBeamEmitterService _tractorBeamEmitters;
        private readonly ISocketService _sockets;

        public string name { get; } = nameof(TractorBeamEmitterUpdateEngine);

        public TractorBeamEmitterUpdateEngine(
            IEntityService entities,
            ISpace space,
            ILogger logger,
            ITractorBeamEmitterService tractorBeamEmitters,
            ISocketService sockets)
        {
            _entities = entities;
            _space = space;
            _logger = logger;
            _tractorBeamEmitters = tractorBeamEmitters;
            _sockets = sockets;
        }

        public void Step(in Step _param)
        {
            foreach (var ((vhids, tacticals, tractorBeamEmitters, count), _) in _entities.QueryEntities<EntityId, Tactical, TractorBeamEmitter>())
            {
                for (int i = 0; i < count; i++)
                {
                    this.UpdateTractorBeamEmitterTractorables(in vhids[i], ref tacticals[i], ref tractorBeamEmitters[i]);
                }
            }
        }

        private void UpdateTractorBeamEmitterTractorables(in EntityId tractorBeamEmitterId, ref Tactical tactical, ref TractorBeamEmitter tractorBeamEmitter)
        {
            ref var filter = ref _tractorBeamEmitters.GetTractorableFilter(tractorBeamEmitterId);
            foreach (var (indices, groupId) in filter)
            {
                var (entityIds, statuses, _) = _entities.QueryEntities<EntityId, EntityStatus>(groupId);

                for (int i = 0; i < indices.count; i++)
                {
                    if (statuses[indices[i]].IsDespawned == false)
                    {
                        ref EntityId tractorableId = ref entityIds[indices[i]];
                        IBody targetBody = _space.GetBody(in tractorableId);

                        EntityId targetId = _entities.GetId(tractorableId.VhId);
                        ref Tree target = ref _entities.QueryById<Tree>(targetId);

                        Location targetHeadChildLocation = _entities.QueryById<Plug>(target.HeadId).Location;

                        if (_sockets.TryGetClosestOpenSocket(tractorBeamEmitterId, tactical.Value, out var openSocketNode))
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
                        ref EntityId tractorableId = ref entityIds[indices[i]];
                        _logger.Warning("{ClassName}::{MethodName} - TractorBeamEmitter = {TractorBeamEmitterId}, Tractorable = {TractorableId}, IsSpawned = false.", nameof(TractorBeamEmitterUpdateEngine), nameof(UpdateTractorBeamEmitterTractorables), tractorBeamEmitterId.VhId, tractorableId.VhId);
                    }
                }
            }
        }
    }
}
