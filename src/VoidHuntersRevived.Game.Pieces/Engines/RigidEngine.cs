using Guppy.Attributes;
using Guppy.Resources.Providers;
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
using VoidHuntersRevived.Common.Pieces.Resources;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Game.Pieces.Events;

namespace VoidHuntersRevived.Game.Pieces.Engines
{
    [AutoLoad]
    internal sealed class RigidEngine : BasicEngine,
        IEventEngine<CreateNode>,
        IEventEngine<DestroyNode>
    {
        private readonly IResourceProvider _resources;
        private readonly ISpace _space;
        private readonly IEntityService _entities;

        public RigidEngine(IResourceProvider resources, ISpace space, IEntityService entities)
        {
            _resources = resources;
            _space = space;
            _entities = entities;
        }

        public void Process(VhId id, CreateNode data)
        {
            IdMap nodeId = _entities.GetIdMap(data.NodeId);

            // Node is not a rigid entity
            if (!this.entitiesDB.TryQueryEntitiesAndIndex<ResourceId<Rigid>>(nodeId.EGID, out uint index, out var rigidIds))
            {
                return;
            }

            var (nodes, _) = this.entitiesDB.QueryEntities<Node>(nodeId.EGID.groupID);

            Rigid rigid = _resources.Get(rigidIds[index])!;
            Node node = nodes[index];
            IBody body = _space.GetOrCreateBody(node.TreeId);
            body.Create(rigid.Shapes[0], nodeId.VhId);
        }

        public void Process(VhId eventId, DestroyNode data)
        {
            try
            {
                IdMap nodeId = _entities.GetIdMap(data.NodeId);

                // Node is not a rigid entity
                if (!this.entitiesDB.TryQueryEntitiesAndIndex<ResourceId<Rigid>>(nodeId.EGID, out uint index, out var rigidIds))
                {
                    return;
                }

                var (nodes, _) = this.entitiesDB.QueryEntities<Node>(nodeId.EGID.groupID);
                Node node = nodes[index];

                if (_space.TryGetBody(node.TreeId, out IBody? body))
                {
                    body.Destroy(nodeId.VhId);
                }
            }
            catch
            {

            }
        }
    }
}
