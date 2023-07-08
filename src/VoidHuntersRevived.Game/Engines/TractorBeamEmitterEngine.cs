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

namespace VoidHuntersRevived.Game.Engines
{
    [AutoLoad]
    internal sealed class TractorBeamEmitterEngine : BasicEngine,
        IEventEngine<TractorBeamEmitter_Activate>,
        IEventEngine<TractorBeamEmitter_SetTarget>
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

            this.Simulation.Publish(eventId.Create(targetTree.HeadId), new TractorBeamEmitter_SetTarget()
            {
                TractorBeamId = shipId.VhId,
                TargetData = _serialization.Serialize(targetTree.HeadId)
            });

            this.Simulation.Publish(DestroyEntity.CreateEvent(targetTreeId.VhId));
        }

        public void Process(VhId eventId, TractorBeamEmitter_SetTarget data)
        {
            VhId targetVhId = eventId.Create(1);

            _treeFactory.Create(targetVhId, EntityTypes.Chain, data.TargetData);
        }
    }
}
