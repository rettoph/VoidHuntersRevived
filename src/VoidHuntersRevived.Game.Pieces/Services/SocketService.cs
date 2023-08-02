﻿using Serilog;
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
using VoidHuntersRevived.Common.Pieces.Factories;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Game.Pieces.Events;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VoidHuntersRevived.Game.Pieces.Services
{
    internal sealed class SocketService : BasicEngine, ISocketService, IQueryingEntitiesEngine,
        IEventEngine<Socket_Attach>,
        IRevertEventEngine<Socket_Attach>,
        IEventEngine<Socket_Detached>
    {
        private readonly ILogger _logger;
        private readonly IEntityService _entities;
        private readonly ITreeFactory _treeFactory;

        public SocketService(IEntityService entities, ITreeFactory treeFactory, ILogger logger)
        {
            _logger = logger;
            _entities = entities;
            _treeFactory = treeFactory;
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

        public void Detach(SocketVhId socketVhId)
        {
            this.Simulation.Publish(
                sender: NameSpace<SocketService>.Instance,
                data: new Socket_Detached()
                {
                    SocketVhId = socketVhId
                });
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

            SocketId socketId = socketNode.Socket.Id;
            EntityId nodeId = _entities.Deserialize(
                seed: socketNode.Node.TreeId.VhId,
                data: _entities.Serialize(tree.HeadId),
                initializer: (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
                {
                    initializer.Init<Coupling>(new Coupling(socketId));
                },
            confirmed: false);

            socketNode.Socket.PlugId = nodeId;
        }

        public void Revert(VhId eventId, Socket_Attach data)
        {
            throw new NotImplementedException();
        }

        public void Process(VhId eventId, Socket_Detached data)
        {
            if (!this.TryGetSocketNode(data.SocketVhId, out SocketNode socketNode))
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - Unable to load SocketNode within node {NodeId}", nameof(SocketService), nameof(Process), nameof(Socket_Detached), data.SocketVhId.NodeVhId.Value);
                return;
            }

            if (socketNode.Socket.PlugId.VhId == default)
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - Socket {SocketNodeId}:{SocketIndex} has no attachment", nameof(SocketService), nameof(Process), nameof(Socket_Detached), socketNode.Socket.Id.NodeId.VhId.Value, socketNode.Socket.Id.Index, socketNode.Socket.PlugId.VhId.Value);
                return;
            }

            //_treeFactory.Create(
            //    vhid: eventId.Create(1)
            //    tree: EntityTypes.Chain)

            socketNode.Socket.PlugId = default;
        }
    }
}
