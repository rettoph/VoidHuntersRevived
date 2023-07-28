using Serilog;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Game.Pieces.Events;

namespace VoidHuntersRevived.Game.Pieces.Services
{
    internal sealed class SocketService : BasicEngine, ISocketService, IQueryingEntitiesEngine,
        IEventEngine<Socket_Spawn>,
        IRevertEventEngine<Socket_Spawn>
    {
        private readonly ILogger _logger;
        private readonly IEntityService _entities;

        public SocketService(IEntityService entities, ILogger logger)
        {
            _logger = logger;
            _entities = entities;
        }

        public SocketNode GetSocketNode(SocketId socketId)
        {
            var nodes = this.entitiesDB.QueryEntitiesAndIndex<Node>(socketId.NodeId.EGID, out uint index);
            var (sockets, _) = this.entitiesDB.QueryEntities<Sockets>(socketId.NodeId.EGID.groupID);


            return new SocketNode(ref sockets[index].Items[socketId.Index], ref nodes[index]);
        }

        public bool TryGetSocketNode(SocketVhId socketVhId, out SocketNode socketNode)
        {
            if(_entities.TryGetId(socketVhId.NodeVhId, out EntityId nodeId))
            {
                socketNode = this.GetSocketNode(new SocketId(nodeId, socketVhId.Index));
                return true;
            }

            socketNode = default;
            return false;
        }

        public void Attach(ref Socket socket, EntityId treeId)
        {
            ref Tree tree = ref this.entitiesDB.QueryEntity<Tree>(treeId.EGID);

            this.Simulation.Publish(
                sender: NameSpace<SocketService>.Instance,
                data: new Socket_Spawn()
                {
                    SocketVhId = socket.Id.VhId,
                    NodeData = _entities.Serialize(tree.HeadId)
                });
        }

        public void Process(VhId eventId, Socket_Spawn data)
        {
            if(!this.TryGetSocketNode(data.SocketVhId, out SocketNode socketNode))
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - Unable to load SocketNode within node {NodeId}", nameof(SocketService), nameof(Process), nameof(Socket_Spawn), data.SocketVhId.NodeVhId.Value);
                return;
            }

            SocketId socketId = socketNode.Socket.Id;
            EntityId nodeId = _entities.Deserialize(
                seed: socketNode.Node.TreeId.VhId,
                data: data.NodeData,
                initializer: (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
                {
                    initializer.Init<Coupling>(new Coupling(socketId));
                },
            confirmed: false);

            socketNode.Socket.PlugId = nodeId;
        }

        public void Revert(VhId eventId, Socket_Spawn data)
        {
            if (!this.TryGetSocketNode(data.SocketVhId, out SocketNode socketNode))
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - Unable to load SocketNode within node {NodeId}", nameof(SocketService), nameof(Revert), nameof(Socket_Spawn), data.SocketVhId.NodeVhId.Value);
                return;
            }

            socketNode.Socket.PlugId = default;
        }
    }
}
