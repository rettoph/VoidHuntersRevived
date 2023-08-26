using Guppy.Attributes;
using Microsoft.Xna.Framework;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.FixedPoint.Extensions;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Ships.Components;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Game.Ships.Services;

namespace VoidHuntersRevived.Game.Ships.Engines
{
    [AutoLoad]
    internal sealed class TractorBeamEmitterUpdateEngine : BasicEngine,
        IStepEngine<Step>
    {
        private readonly IEntityService _entities;
        private readonly ISpace _space;
        private readonly ILogger _logger;
        private readonly TractorBeamEmitterService _tractorBeamEmitterService;

        public string name { get; } = nameof(TractorBeamEmitterUpdateEngine);

        public TractorBeamEmitterUpdateEngine(
            IEntityService entities, 
            ISpace space,
            ILogger logger,
            TractorBeamEmitterService tractorBeamEmitterService)
        {
            _entities = entities;
            _space = space;
            _logger = logger;
            _tractorBeamEmitterService = tractorBeamEmitterService;
        }

        public void Step(in Step _param)
        {
            foreach (var ((vhids, tacticals, tractorBeamEmitters, count), _) in _entities.QueryEntities<EntityId, Tactical, TractorBeamEmitter>())
            {
                for (int i = 0; i < count; i++)
                {
                    this.UpdateTractorBeamEmitterTarget(in vhids[i], ref tacticals[i], ref tractorBeamEmitters[i]);
                }
            }
        }

        private void UpdateTractorBeamEmitterTarget(in EntityId shipId, ref Tactical tactical, ref TractorBeamEmitter tractorBeamEmitter)
        {
            if(tractorBeamEmitter.Active == false)
            {
                return;
            }

            IBody targetBody = _space.GetBody(in tractorBeamEmitter.TargetId.VhId);

            EntityId targetId = _entities.GetId(tractorBeamEmitter.TargetId.VhId);
            ref Tree target = ref _entities.QueryById<Tree>(targetId);

            Location targetHeadChildLocation = _entities.QueryById<Plug>(target.HeadId).Location;
            
            if (_tractorBeamEmitterService.TryGetClosestOpenSocket(shipId, tactical.Value, out var openSocketNode))
            {
                FixMatrix potentialTransformation = targetHeadChildLocation.Transformation.Invert() * openSocketNode.Transformation;
                FixVector2 potentialPosition = FixVector2.Transform(FixVector2.Zero, potentialTransformation);

                targetBody.SetTransform(potentialPosition, potentialTransformation.Radians());
            
                return;
            }

            FixVector2 targetHeadChildNodePosition = FixVector2.Transform(FixVector2.Zero, targetHeadChildLocation.Transformation * FixMatrix.CreateRotationZ(targetBody.Rotation));
            targetBody.SetTransform(tactical.Value - targetHeadChildNodePosition, targetBody.Rotation);
        }
    }
}
