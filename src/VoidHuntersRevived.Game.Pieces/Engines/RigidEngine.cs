using Guppy.Attributes;
using Guppy.Resources.Providers;
using Serilog;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Game.Pieces.Events;

namespace VoidHuntersRevived.Game.Pieces.Engines
{
    [AutoLoad]
    internal sealed class RigidEngine : BasicEngine,
        IEventEngine<Node_Create>,
        IEventEngine<Node_Destroy>
    {
        private readonly ISpace _space;
        private readonly IEntityService _entities;
        private readonly ILogger _logger;

        public RigidEngine(ISpace space, IEntityService entities, ILogger logger)
        {
            _space = space;
            _entities = entities;
            _logger = logger;
        }

        public void Process(VhId id, Node_Create data)
        {
            EntityId nodeId = _entities.GetId(data.NodeId);

            // Node is not a rigid entity
            if (!this.entitiesDB.TryQueryEntitiesAndIndex<Rigid>(nodeId.EGID, out uint index, out var rigids))
            {
                return;
            }

            var (nodes, count) = this.entitiesDB.QueryEntities<Node>(nodeId.EGID.groupID);

            Rigid rigid = rigids[index];
            Node node = nodes[index];
            IBody body = _space.GetOrCreateBody(node.TreeId.VhId);

            body.Create(nodeId.VhId, rigid.Shapes[0], node.LocalTransformation);
        }

        public void Process(VhId eventId, Node_Destroy data)
        {
            EntityId nodeId = _entities.GetId(data.NodeId);

            // Node is not a rigid entity
            if (!this.entitiesDB.TryQueryEntitiesAndIndex<Rigid>(nodeId.EGID, out uint index, out var _))
            {
                return;
            }

            var (nodes, _) = this.entitiesDB.QueryEntities<Node>(nodeId.EGID.groupID);
            Node node = nodes[index];

            if (_space.TryGetBody(node.TreeId.VhId, out IBody? body))
            {
                body.Destroy(nodeId.VhId);
            }
            else
            {
                _logger.Verbose("{ClassName}::{MethodName}<{EventName}> - Unable to find body {BodyId} while attempting to remove fixture {FixtureId}", nameof(RigidEngine), nameof(Process), nameof(Node_Destroy), node.TreeId.VhId.Value, nodeId.VhId.Value);
            }
        }
    }
}
