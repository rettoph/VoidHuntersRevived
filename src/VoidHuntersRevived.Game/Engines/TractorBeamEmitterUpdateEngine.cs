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
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Game.Components;
using VoidHuntersRevived.Game.Services;

namespace VoidHuntersRevived.Game.Engines
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
            var groups = this.entitiesDB.FindGroups<Tactical, TractorBeamEmitter>();
            foreach (var ((vhids, tacticals, tractorBeamEmitters, count), _) in this.entitiesDB.QueryEntities<EntityId, Tactical, TractorBeamEmitter>(groups))
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

            IBody targetBody = _space.GetBody(in tractorBeamEmitter.TargetVhId);

            EntityId targetId = _entities.GetId(tractorBeamEmitter.TargetVhId);
            ref Tree target = ref this.entitiesDB.QueryEntity<Tree>(targetId.EGID);

            Location targetHeadChildLocation = this.entitiesDB.QueryEntity<Joints>(target.HeadId.EGID).Child;
            
            if (_tractorBeamEmitterService.TryGetClosestOpenJoint(shipId, tactical.Value, out var openNodeJoint))
            {
                FixMatrix potentialTransformation = targetHeadChildLocation.Transformation * openNodeJoint.Transformation;
                FixVector2 potentialPosition = FixVector2.Transform(FixVector2.Zero, potentialTransformation);
            
                targetBody.SetTransform(potentialPosition, Fix64.Pi - openNodeJoint.Transformation.Radians());
            
                return;
            }

            FixVector2 targetHeadChildNodePosition = FixVector2.Transform(FixVector2.Zero, targetHeadChildLocation.Transformation * FixMatrix.CreateRotationZ(targetBody.Rotation));
            targetBody.SetTransform(tactical.Value - targetHeadChildNodePosition, targetBody.Rotation);
        }
    }
}
