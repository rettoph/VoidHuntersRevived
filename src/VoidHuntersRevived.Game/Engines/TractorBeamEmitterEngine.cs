using Guppy.Attributes;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Events;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Game.Components;
using VoidHuntersRevived.Game.Events;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Factories;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Game.Services;
using Svelto.DataStructures;
using VoidHuntersRevived.Game.Enums;
using VoidHuntersRevived.Domain.Common.Components;
using Serilog;

namespace VoidHuntersRevived.Game.Engines
{
    [AutoLoad]
    internal sealed class TractorBeamEmitterEngine : BasicEngine,
        IEventEngine<TractorBeamEmitter_TryActivate>,
        IEventEngine<TractorBeamEmitter_TryDeactivate>,
        IEventEngine<TractorBeamEmitter_Activate>,
        IRevertEventEngine<TractorBeamEmitter_Activate>,
        IEventEngine<TractorBeamEmitter_Deactivate>,
        IStepEngine<Step>
    {
        public static readonly Fix64 QueryRadius = (Fix64)5;

        private readonly IEntityIdService _entities;
        private readonly IEntitySerializationService _serialization;
        private readonly ISpace _space;
        private readonly ITreeFactory _treeFactory;
        private readonly ILogger _logger;
        private readonly TractorBeamEmitterService _tractorBeamEmitterService;

        public TractorBeamEmitterEngine(
            IEntityIdService entities, 
            IEntitySerializationService serialization, 
            ISpace space, 
            ITreeFactory treeFactory,
            ILogger logger,
            TractorBeamEmitterService tractorBeamEmitterService)
        {
            _entities = entities;
            _serialization = serialization;
            _space = space;
            _treeFactory = treeFactory;
            _logger = logger;
            _tractorBeamEmitterService = tractorBeamEmitterService;
        }

        public string name { get; } = nameof(TractorBeamEmitterEngine);


        public void Process(VhId eventId, TractorBeamEmitter_TryActivate data)
        {
            if(!_entities.TryGetId(data.TargetVhId, out EntityId targetNodeId) || targetNodeId.Destroyed)
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - TargetVhId {TargetId} not found.", nameof(TractorBeamEmitterEngine), nameof(Process), nameof(TractorBeamEmitter_TryActivate), data.TargetVhId.Value);
                return;
            }

            Node targetNode = this.entitiesDB.QueryEntity<Node>(targetNodeId.EGID);

            if (!_entities.TryGetId(targetNode.TreeId, out EntityId targetTreeId) || targetTreeId.Destroyed)
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - TreeId {TargetTreeId} not found.", nameof(TractorBeamEmitterEngine), nameof(Process), nameof(TractorBeamEmitter_TryActivate), targetNode.TreeId.Value);
                return;
            }
            Tree targetTree = this.entitiesDB.QueryEntity<Tree>(targetTreeId.EGID);

            this.Simulation.Publish(eventId, new TractorBeamEmitter_Activate()
            {
                TractorBeamEmitterVhId = data.ShipVhId,
                TargetData = _serialization.Serialize(targetTree.HeadId)
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
                    ShipVhId = data.TractorBeamEmitterVhId
                });
            }

            EntityId cloneId = _treeFactory.Create(
                vhid: eventId.Create(1),
                tree: EntityTypes.Chain,
                pieces: data.TargetData,
                initializer: (ref EntityInitializer initializer) =>
                {
                    initializer.Init<Tractorable>(new Tractorable()
                    {
                        IsTractored = true
                    });
                });

            tractorBeamEmitter.TargetVhId = cloneId.VhId;
            tractorBeamEmitter.Active = true;
            tactical.AddUse();

            _logger.Verbose("{ClassName}::{MethodName}<{GenericTypeName}> - TractorBeam {TractorBeamId} has selected {TargetId}", nameof(TractorBeamEmitterEngine), nameof(Process), nameof(TractorBeamEmitter_Activate), tractorBeamEmitterId.VhId.Value, tractorBeamEmitter.TargetVhId.Value);
        }

        public void Revert(VhId eventId, TractorBeamEmitter_Activate data)
        {
            EntityId tractorBeamEmitterId = _entities.GetId(data.TractorBeamEmitterVhId);

            var tractorBeamEmitters = this.entitiesDB.QueryEntitiesAndIndex<TractorBeamEmitter>(tractorBeamEmitterId.EGID, out uint index);
            ref TractorBeamEmitter tractorBeamEmitter = ref tractorBeamEmitters[index];

            if (tractorBeamEmitter.TargetVhId.Value != eventId.Create(1).Value)
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - Target is no longer {OldTargetId}, now {NewTargetId}", nameof(TractorBeamEmitterEngine), nameof(Revert), nameof(TractorBeamEmitter_Activate), eventId.Create(1).Value, tractorBeamEmitter.TargetVhId.Value);
                return;
            }

            tractorBeamEmitter.Active = false;
        }

        public void Process(VhId eventId, TractorBeamEmitter_TryDeactivate data)
        {
            EntityId tractorBeamEmitterId = _entities.GetId(data.ShipVhId);
            ref TractorBeamEmitter tractorBeamEmitter = ref entitiesDB.QueryMappedEntities<TractorBeamEmitter>(tractorBeamEmitterId.EGID.groupID).Entity(tractorBeamEmitterId.EGID.entityID);

            this.Simulation.Publish(eventId, new TractorBeamEmitter_Deactivate()
            {
                TractorBeamEmitterVhId = data.ShipVhId,
                TargetVhId = tractorBeamEmitter.TargetVhId
            });
        }

        public void Process(VhId eventId, TractorBeamEmitter_Deactivate data)
        {
            EntityId tractorBeamEmitterId = _entities.GetId(data.TractorBeamEmitterVhId);
            var tractorBeamEmitters = this.entitiesDB.QueryEntitiesAndIndex<TractorBeamEmitter>(tractorBeamEmitterId.EGID, out uint tractorBeamEmitterIndex);
            var (tacticals, _) = this.entitiesDB.QueryEntities<Tactical>(tractorBeamEmitterId.EGID.groupID);

            ref TractorBeamEmitter tractorBeamEmitter = ref tractorBeamEmitters[tractorBeamEmitterIndex];
            ref Tactical tactical = ref tacticals[tractorBeamEmitterIndex];

            if (tractorBeamEmitter.TargetVhId.Value != data.TargetVhId.Value)
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - TargetVhId has changed from {OldTargetId} to {NewTargetId}", nameof(TractorBeamEmitterEngine), nameof(Process), nameof(TractorBeamEmitter_Deactivate), data.TargetVhId.Value, tractorBeamEmitter.TargetVhId.Value);

                return;
            }

            // Ensure the emitter is deactivated no matter what
            tractorBeamEmitter.Active = false;
            tactical.RemoveUse();

            if (!_entities.TryGetId(tractorBeamEmitter.TargetVhId, out EntityId targetId))
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - TargetVhId {TargetVhId} map not found", nameof(TractorBeamEmitterEngine), nameof(Process), nameof(TractorBeamEmitter_TryDeactivate), tractorBeamEmitter.TargetVhId.Value);

                return;
            }

            if(!this.entitiesDB.TryQueryEntitiesAndIndex<Tractorable>(targetId.EGID, out uint tractorableIndex, out var tractorables))
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - Tractorable entity not found", nameof(TractorBeamEmitterEngine), nameof(Process), nameof(TractorBeamEmitter_TryDeactivate), tractorBeamEmitter.TargetVhId.Value);

                return;
            }

            tractorables[tractorableIndex].IsTractored = false;
        }

        public void Step(in Step _param)
        {
            var groups = this.entitiesDB.FindGroups<Tactical, TractorBeamEmitter>();
            foreach (var ((tacticals, tractorBeamEmitters, count), groupId) in this.entitiesDB.QueryEntities<Tactical, TractorBeamEmitter>(groups))
            {
                for (int i = 0; i < count; i++)
                {
                    this.UpdateTractorBeamEmitterTarget(ref tacticals[i], ref tractorBeamEmitters[i]);
                }
            }
        }

        private void UpdateTractorBeamEmitterTarget(ref Tactical tactical, ref TractorBeamEmitter tractorBeamEmitter)
        {
            if(tractorBeamEmitter.Active == false)
            {
                return;
            }

            IBody targetBody = _space.GetBody(in tractorBeamEmitter.TargetVhId);

            targetBody.SetTransform(tactical.Value, targetBody.Rotation);
        }
    }
}
