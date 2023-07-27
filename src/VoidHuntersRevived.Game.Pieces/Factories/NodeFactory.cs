using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Factories;

namespace VoidHuntersRevived.Game.Pieces.Factories
{
    internal sealed class NodeFactory : INodeFactory, IQueryingEntitiesEngine
    {
        private readonly IEntityService _entities;

        public NodeFactory(IEntityService entities)
        {
            _entities = entities;
        }

        public EntitiesDB entitiesDB { get; set; } = null!;

        public void Ready()
        {
            // throw new NotImplementedException();
        }

        public EntityId Create(ref Socket socket, EntityData nodes)
        {
            ref Node parent = ref this.entitiesDB.QueryEntity<Node>(socket.Id.NodeId.EGID);
            SocketId socketId = socket.Id;
            EntityId nodeId = _entities.Deserialize(
                seed: parent.TreeId.VhId, 
                data: nodes, 
                initializer: (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
                {
                    initializer.Init<Coupling>(new Coupling(socketId));
                }, 
                confirmed: false);

            socket.PlugId = nodeId;

            return nodeId;
        }
    }
}
