using Serilog;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
using VoidHuntersRevived.Common.Simulations.Exceptions;
using VoidHuntersRevived.Game.Common;
using VoidHuntersRevived.Game.Pieces.Events;

namespace VoidHuntersRevived.Game.Pieces.Services
{
    internal sealed class SocketService : BasicEngine, ISocketService,
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

            ref Node node = ref _entities.QueryById<Node>(socketId.NodeId, out GroupIndex groupIndex);
            ref Sockets sockets = ref _entities.QueryByGroupIndex<Sockets>(groupIndex);


            return new SocketNode(ref sockets.Items[socketId.Index], ref node);
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

        public ref EntityFilterCollection GetCouplingFilter(SocketId socketId)
        {
            return ref _entities.GetFilter<Coupling>(socketId.NodeId, socketId.FilterContextId);
        }

        public void Attach(SocketId socketId, EntityId nodeId)
        {
            this.Simulation.Publish(
                sender: NameSpace<SocketService>.Instance,
                data: new Socket_Attach()
                {
                    SocketVhId = socketId.VhId,
                    NodeData = _entities.Serialize(nodeId)
                });
        }

        public bool TryDetach(EntityId couplingId, EntityInitializerDelegate initializer, out EntityId cloneId)
        {
            VhId cloneVhId = NameSpace<Socket_Detached>.Instance.Create(couplingId.VhId);

            this.Simulation.Publish(
                sender: NameSpace<SocketService>.Instance,
                data: new Socket_Detached()
                {
                    CouplingVhId = couplingId.VhId,
                    CloneVhId = cloneVhId,
                    Initializer = initializer
                });

            return _entities.TryGetId(cloneVhId, out cloneId);
        }

        public void Process(VhId eventId, Socket_Attach data)
        {
            try
            {
                if (!this.TryGetSocketNode(data.SocketVhId, out SocketNode socketNode))
                {
                    throw new SimulationOutOfSyncException($"Unable to load {nameof(SocketNode)} within {nameof(Node)} {data.SocketVhId.NodeVhId.Value}");
                }

                SocketVhId socketVhId = socketNode.Socket.Id.VhId;
                EntityId nodeId = _entities.Deserialize(
                    seed: socketNode.Node.TreeId.VhId,
                    data: data.NodeData,
                    initializer: (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
                    {
                        initializer.Init<Coupling>(new Coupling(
                            socketId: new SocketId(
                                nodeId: entities.GetId(socketVhId.NodeVhId),
                                index: socketVhId.Index))
                            );
                    },
                    confirmed: false);
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "{ClassName}::{MethodName}<{GenericTypeName}> - Exception thrown", nameof(SocketService), nameof(Process), nameof(Socket_Attach));
                throw new SimulationOutOfSyncException(ex.Message, ex);
            }

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

            if (!_entities.TryQueryById<Coupling>(couplingId, out Coupling coupling))
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

            _entities.Despawn(couplingId);
        }
    }
}
