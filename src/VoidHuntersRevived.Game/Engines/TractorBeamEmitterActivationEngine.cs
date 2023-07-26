using Guppy.Attributes;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Events;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Factories;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Game.Components;
using VoidHuntersRevived.Game.Events;
using VoidHuntersRevived.Common.Entities.Services;
using Serilog;

namespace VoidHuntersRevived.Game.Engines
{
    [AutoLoad]
    internal class TractorBeamEmitterActivationEngine : BasicEngine,
        IEventEngine<TractorBeamEmitter_TryActivate>,
        IEventEngine<TractorBeamEmitter_Activate>,
        IRevertEventEngine<TractorBeamEmitter_Activate>
    {
        private readonly IEntityService _entities;
        private readonly ITreeFactory _treeFactory;
        private readonly ILogger _logger;

        public TractorBeamEmitterActivationEngine(
            IEntityService entities, 
            ITreeFactory treeFactory,
            ILogger logger)
        {
            _entities = entities;
            _treeFactory = treeFactory;
            _logger = logger;
        }

        public void Process(VhId eventId, TractorBeamEmitter_TryActivate data)
        {
            if (!_entities.TryGetId(data.TargetVhId, out EntityId targetNodeId) || _entities.GetState(targetNodeId) != EntityState.Spawned)
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - TargetVhId {TargetId} not found.", nameof(TractorBeamEmitterActivationEngine), nameof(Process), nameof(TractorBeamEmitter_TryActivate), data.TargetVhId.Value);
                return;
            }

            Node targetNode = this.entitiesDB.QueryEntity<Node>(targetNodeId.EGID);

            if (!_entities.TryGetId(targetNode.TreeId.VhId, out EntityId targetTreeId) || _entities.GetState(targetTreeId) != EntityState.Spawned)
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - TreeId {TargetTreeId} not found.", nameof(TractorBeamEmitterActivationEngine), nameof(Process), nameof(TractorBeamEmitter_TryActivate), targetNode.TreeId.VhId.Value);
                return;
            }
            Tree targetTree = this.entitiesDB.QueryEntity<Tree>(targetTreeId.EGID);

            this.Simulation.Publish(eventId, new TractorBeamEmitter_Activate()
            {
                TractorBeamEmitterVhId = data.ShipVhId,
                TargetData = _entities.Serialize(targetTree.HeadId)
            });

            this.Simulation.Publish(eventId, new DespawnEntity()
            {
                VhId = targetTreeId.VhId
            });
        }

        public void Process(VhId eventId, TractorBeamEmitter_Activate data)
        {
            EntityId tractorBeamEmitterId = _entities.GetId(data.TractorBeamEmitterVhId);
            var tractorBeamEmitters = this.entitiesDB.QueryEntitiesAndIndex<TractorBeamEmitter>(tractorBeamEmitterId.EGID, out uint index);
            var (tacticals, _) = this.entitiesDB.QueryEntities<Tactical>(tractorBeamEmitterId.EGID.groupID);

            ref TractorBeamEmitter tractorBeamEmitter = ref tractorBeamEmitters[index];
            ref Tactical tactical = ref tacticals[index];

            if (tractorBeamEmitter.Active)
            {
                this.Simulation.Publish(eventId, new TractorBeamEmitter_TryDeactivate()
                {
                    ShipVhId = data.TractorBeamEmitterVhId,
                    AttachTo = null
                });
            }

            EntityId cloneId = _treeFactory.Create(
                vhid: eventId.Create(1),
                tree: EntityTypes.Chain,
                pieces: data.TargetData,
                initializer: (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
                {
                    initializer.Init<Tractorable>(new Tractorable()
                    {
                        IsTractored = true
                    });
                });

            tractorBeamEmitter.TargetId = cloneId;
            tractorBeamEmitter.Active = true;
            tactical.AddUse();

            _logger.Verbose("{ClassName}::{MethodName}<{GenericTypeName}> - TractorBeam {TractorBeamId} has selected {TargetId}", nameof(TractorBeamEmitterActivationEngine), nameof(Process), nameof(TractorBeamEmitter_Activate), tractorBeamEmitterId.VhId.Value, tractorBeamEmitter.TargetId.VhId.Value);
        }

        public void Revert(VhId eventId, TractorBeamEmitter_Activate data)
        {
            EntityId tractorBeamEmitterId = _entities.GetId(data.TractorBeamEmitterVhId);

            var tractorBeamEmitters = this.entitiesDB.QueryEntitiesAndIndex<TractorBeamEmitter>(tractorBeamEmitterId.EGID, out uint index);
            ref TractorBeamEmitter tractorBeamEmitter = ref tractorBeamEmitters[index];

            if (tractorBeamEmitter.TargetId.VhId.Value != eventId.Create(1).Value)
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - Target is no longer {OldTargetId}, now {NewTargetId}", nameof(TractorBeamEmitterActivationEngine), nameof(Revert), nameof(TractorBeamEmitter_Activate), eventId.Create(1).Value, tractorBeamEmitter.TargetId.VhId.Value);
                return;
            }

            tractorBeamEmitter.Active = false;
        }
    }
}
