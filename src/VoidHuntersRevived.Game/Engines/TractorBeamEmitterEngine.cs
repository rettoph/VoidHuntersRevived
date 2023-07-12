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

namespace VoidHuntersRevived.Game.Engines
{
    [AutoLoad]
    internal sealed class TractorBeamEmitterEngine : BasicEngine,
        IEventEngine<TractorBeamEmitter_Activate>,
        IEventEngine<TractorBeamEmitter_Deactivate>,
        IStepEngine<Step>
    {
        public static readonly Fix64 QueryRadius = (Fix64)5;

        private readonly IEntityService _entities;
        private readonly IEntitySerializationService _serialization;
        private readonly ISpace _space;
        private readonly ITreeFactory _treeFactory;
        private readonly TractorBeamEmitterService _tractorBeamEmitterService;

        public TractorBeamEmitterEngine(
            IEntityService entities, 
            IEntitySerializationService serialization, 
            ISpace space, 
            ITreeFactory treeFactory,
            TractorBeamEmitterService tractorBeamEmitterService)
        {
            _entities = entities;
            _serialization = serialization;
            _space = space;
            _treeFactory = treeFactory;
            _tractorBeamEmitterService = tractorBeamEmitterService;
        }

        public string name { get; } = nameof(TractorBeamEmitterEngine);


        public void Process(VhId eventId, TractorBeamEmitter_Activate data)
        {
            IdMap shipId = _entities.GetIdMap(data.ShipVhId);
            ref TractorBeamEmitter tractorBeamEmitter = ref entitiesDB.QueryMappedEntities<TractorBeamEmitter>(shipId.EGID.groupID).Entity(shipId.EGID.entityID);

            if (tractorBeamEmitter.Active)
            {
                return;
            }

            IdMap targetNodeId = _entities.GetIdMap(data.TargetVhId);
            Node targetNode = this.entitiesDB.QueryEntity<Node>(targetNodeId.EGID);

            IdMap targetTreeId = _entities.GetIdMap(targetNode.TreeId);
            Tree targetTree = this.entitiesDB.QueryEntity<Tree>(targetTreeId.EGID);

            IdMap cloneId = _treeFactory.Create(
                vhid: eventId.Create(targetNode.TreeId), 
                tree: EntityTypes.Chain, 
                pieces: _serialization.Serialize(targetTree.HeadId), 
                initializer: (ref EntityInitializer initializer) =>
                {
                    initializer.Init<Tractorable>(new Tractorable()
                    {
                        IsTractored = true
                    });
                });

            tractorBeamEmitter.TargetEGID = cloneId.EGID;
            tractorBeamEmitter.Active = true;

            this.Simulation.Publish(DestroyEntity.CreateEvent(targetTreeId.VhId));
        }

        public void Process(VhId eventId, TractorBeamEmitter_Deactivate data)
        {
            IdMap shipId = _entities.GetIdMap(data.ShipVhId);
            ref TractorBeamEmitter tractorBeamEmitter = ref entitiesDB.QueryMappedEntities<TractorBeamEmitter>(shipId.EGID.groupID).Entity(shipId.EGID.entityID);

            IdMap targetId = _entities.GetIdMap(tractorBeamEmitter.TargetEGID);

            ref Tractorable target = ref entitiesDB.QueryEntity<Tractorable>(targetId.EGID);

            target.IsTractored = false;
            tractorBeamEmitter.Active = false;
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
