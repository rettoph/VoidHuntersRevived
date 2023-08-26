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
using VoidHuntersRevived.Game.Common;
using VoidHuntersRevived.Game.Pieces.Events;

namespace VoidHuntersRevived.Game.Pieces.Services
{
    internal sealed class SocketService : BasicEngine, ISocketService, IQueryingEntitiesEngine,
        IEventEngine<Socket_Attach>,
        IRevertEventEngine<Socket_Attach>,
        IEventEngine<Socket_Detached>
    {
        private readonly ILogger _logger;
        private readonly IEntityService _entities;
        private readonly ITreeService _trees;

        public SocketService(IEntityService entities, ITreeService trees, ILogger logger)
        {
            _logger = logger;
            _entities = entities;
            _trees = trees;
        }

        public SocketNode GetSocketNode(SocketId socketId)
        {
            _logger.Verbose("{ClassName}::{MethodName} - Locating {NodeId}:{SocketIndex} - Node EGID {EntityId}:{GroupId}", nameof(SocketService), nameof(GetSocketNode), socketId.NodeId.VhId.Value, socketId.Index, socketId.NodeId.EGID.entityID, socketId.NodeId.EGID.groupID);

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

        public void Attach(SocketVhId socketVhId, VhId treeVhId)
        {
            this.Simulation.Publish(
                sender: NameSpace<SocketService>.Instance,
                data: new Socket_Attach()
                {
                    SocketVhId = socketVhId,
                    TreeVhId = treeVhId
                });
        }

        public bool TryDetach(EntityId couplingId, EntityInitializerDelegate initializer, out EntityId cloneId)
        {
            VhId clonoeVhId = NameSpace<Socket_Detached>.Instance.Create(couplingId.VhId);

            this.Simulation.Publish(
                sender: NameSpace<SocketService>.Instance,
                data: new Socket_Detached()
                {
                    CouplingVhId = couplingId.VhId,
                    CloneVhId = clonoeVhId,
                    Initializer = initializer
                });

            return _entities.TryGetId(clonoeVhId, out cloneId);
        }

        public void Process(VhId eventId, Socket_Attach data)
        {
            if(!this.TryGetSocketNode(data.SocketVhId, out SocketNode socketNode))
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - Unable to load SocketNode within node {NodeId}", nameof(SocketService), nameof(Process), nameof(Socket_Attach), data.SocketVhId.NodeVhId.Value);
                return;
            }

            if(socketNode.Socket.PlugId.VhId != default)
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - Socket {SocketNodeId}:{SocketIndex} already has existing attachment with {NodeId}", nameof(SocketService), nameof(Process), nameof(Socket_Attach), socketNode.Socket.Id.NodeId.VhId.Value, socketNode.Socket.Id.Index, socketNode.Socket.PlugId.VhId.Value);
                return;
            }

            if (!_entities.TryGetId(data.TreeVhId, out EntityId treeId))
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - TreeVhId {TreeVhId} not found.", nameof(SocketService), nameof(Process), nameof(Socket_Attach), data.TreeVhId.Value);
                return;
            }

            ref Tree tree = ref this.entitiesDB.QueryEntity<Tree>(treeId.EGID);

            SocketVhId socketVhId = socketNode.Socket.Id.VhId;
            EntityId nodeId = _entities.Deserialize(
                seed: socketNode.Node.TreeId.VhId,
                data: _entities.Serialize(tree.HeadId),
                initializer: (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
                {
                    initializer.Init<Coupling>(new Coupling(
                        socketId: new SocketId(
                            nodeId: entities.GetId(socketVhId.NodeVhId),
                            index: socketVhId.Index))
                        );
                },
            confirmed: false);

            _entities.Despawn(treeId);

            socketNode.Socket.PlugId = nodeId;
        }

        public void Revert(VhId eventId, Socket_Attach data)
        {
            throw new NotImplementedException();
        }

        public void Process(VhId eventId, Socket_Detached data)
        {
            if(!_entities.TryGetId(data.CouplingVhId, out EntityId couplingId))
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - Unable to find EntityId {CouplingVhId}", nameof(SocketService), nameof(Process), nameof(Socket_Detached), data.CouplingVhId.Value);
                return;
            }

            if(!this.entitiesDB.TryGetEntity<Coupling>(couplingId.EGID, out Coupling coupling))
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - Unable to find Coupling for {EntityVhId}", nameof(SocketService), nameof(Process), nameof(Socket_Detached), data.CouplingVhId.Value);
                return;
            }

            _entities.Flush();

            SocketNode socketNode = this.GetSocketNode(coupling.SocketId);

            _trees.Spawn(
                vhid: data.CloneVhId,
                tree: EntityTypes.Chain,
                nodes: _entities.Serialize(couplingId),
                initializer: data.Initializer);

            _entities.Despawn(socketNode.Socket.PlugId);
            socketNode.Socket.PlugId = default;
        }
    }
}
