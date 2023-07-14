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
        IStepEngine<Step>
    {
        public static readonly Fix64 QueryRadius = (Fix64)5;

        private readonly IEntityService _entities;
        private readonly IEntitySerializationService _serialization;
        private readonly ISpace _space;
        private readonly ITreeFactory _treeFactory;
        private readonly ILogger _logger;
        private readonly TractorBeamEmitterService _tractorBeamEmitterService;

        public TractorBeamEmitterEngine(
            IEntityService entities, 
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
            if(!_entities.TryGetIdMap(data.TargetVhId, out IdMap targetNodeId) || targetNodeId.Destroyed)
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - {TargetIdPropertyName} {TargetId} not found.", nameof(TractorBeamEmitterEngine), nameof(Process), nameof(TractorBeamEmitter_TryActivate), nameof(TractorBeamEmitter_TryActivate.TargetVhId), data.TargetVhId.Value);
                return;
            }

            Node targetNode = this.entitiesDB.QueryEntity<Node>(targetNodeId.EGID);

            if (!_entities.TryGetIdMap(targetNode.TreeId, out IdMap targetTreeId) || targetTreeId.Destroyed)
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - {TargetTreePropertyName} {TargetTreeId} not found.", nameof(TractorBeamEmitterEngine), nameof(Process), nameof(TractorBeamEmitter_TryActivate), nameof(Node.TreeId), targetNode.TreeId.Value);
                return;
            }
            Tree targetTree = this.entitiesDB.QueryEntity<Tree>(targetTreeId.EGID);

            this.Simulation.Publish(TractorBeamEmitter_Activate.NameSpace.Create(eventId).Create(targetTreeId.VhId), new TractorBeamEmitter_Activate()
            {
                TractorBeamEmitterVhId = data.ShipVhId,
                TargetData = _serialization.Serialize(targetTree.HeadId)
            });

            this.Simulation.Publish(DestroyEntity.CreateEvent(targetTreeId.VhId));
        }

        public void Process(VhId eventId, TractorBeamEmitter_Activate data)
        {
            IdMap tractorBeamEmitterVhId = _entities.GetIdMap(data.TractorBeamEmitterVhId);
            ref TractorBeamEmitter tractorBeamEmitter = ref entitiesDB.QueryMappedEntities<TractorBeamEmitter>(tractorBeamEmitterVhId.EGID.groupID).Entity(tractorBeamEmitterVhId.EGID.entityID);

            if (tractorBeamEmitter.Active)
            {
                this.Simulation.Publish(TractorBeamEmitter_TryDeactivate.NameSpace.Create(eventId), new TractorBeamEmitter_TryDeactivate()
                {
                    ShipVhId = data.TractorBeamEmitterVhId
                });
            }

            IdMap cloneId = _treeFactory.Create(
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

            tractorBeamEmitter.TargetEGID = cloneId.EGID;
            tractorBeamEmitter.Active = true;
        }

        public void Revert(VhId eventId, TractorBeamEmitter_Activate data)
        {
            IdMap tractorBeamEmitterVhId = _entities.GetIdMap(data.TractorBeamEmitterVhId);
            ref TractorBeamEmitter tractorBeamEmitter = ref entitiesDB.QueryMappedEntities<TractorBeamEmitter>(tractorBeamEmitterVhId.EGID.groupID).Entity(tractorBeamEmitterVhId.EGID.entityID);

            if(!_entities.TryGetIdMap(tractorBeamEmitter.TargetEGID, out IdMap targetId))
            {
                return;
            }

            if(targetId.VhId.Value != eventId.Create(1).Value)
            {
                return;
            }

            tractorBeamEmitter.Active = false;
        }

        public void Process(VhId eventId, TractorBeamEmitter_TryDeactivate data)
        {
            IdMap shipId = _entities.GetIdMap(data.ShipVhId);
            ref TractorBeamEmitter tractorBeamEmitter = ref entitiesDB.QueryMappedEntities<TractorBeamEmitter>(shipId.EGID.groupID).Entity(shipId.EGID.entityID);

            if(!_entities.TryGetIdMap(tractorBeamEmitter.TargetEGID, out IdMap targetId))
            {
                return;
            }

            try
            {
                ref Tractorable target = ref entitiesDB.QueryEntity<Tractorable>(targetId.EGID);

                target.IsTractored = false;
                tractorBeamEmitter.Active = false;
            }
            catch(Exception e)
            {
                _logger.Error(e, "{ClassName}::{MethodName}<{GenericTypeName}> - Target {TargetId} not found, deactivation request sent in the same frame as activation?", nameof(TractorBeamEmitterEngine), nameof(Process), nameof(TractorBeamEmitter_TryDeactivate), targetId.VhId.Value);
            }

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

            EntityVhId vhid = this.entitiesDB.QueryEntity<EntityVhId>(tractorBeamEmitter.TargetEGID);
            IBody targetBody = _space.GetBody(in vhid.Value);

            targetBody.SetTransform(tactical.Value, targetBody.Rotation);
        }
    }
}
