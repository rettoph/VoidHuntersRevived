using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Pieces.Descriptors;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities.Options;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Game.Pieces.Services
{
    internal partial class SocketService : ISocketService
    {
        public EntityId Spawn(SocketNode socketNode, VhId nodeVhId, IEntityType<PieceDescriptor> node, EntityInitializerDelegate? initializerDelegate = null)
        {
            TeamId teamId = _entities.QueryById<TeamId>(socketNode.Node.TreeId);
            SocketVhId socketVhId = socketNode.Socket.Id.VhId;
            VhId treeId = socketNode.Node.TreeId.VhId;

            return _entities.Spawn(node, nodeVhId, teamId, (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
            {
                initializer.Init(new Node(id, entities.GetId(treeId)));
                initializer.Init<Coupling>(new Coupling(
                socketId: new SocketId(
                    nodeId: entities.GetId(socketVhId.NodeVhId),
                    index: socketVhId.Index))
                );

                initializerDelegate?.Invoke(entities, ref initializer, in id);
            });
        }

        public EntityId Spawn(SocketNode socketNode, EntityData nodes, EntityInitializerDelegate? initializerDelegate = null)
        {
            TeamId teamId = _entities.QueryById<TeamId>(socketNode.Node.TreeId);
            SocketVhId socketVhId = socketNode.Socket.Id.VhId;

            EntityId nodeId = _entities.Deserialize(
                options: new DeserializationOptions
                {
                    Seed = HashBuilder<SocketService,  SocketVhId>.Instance.Calculate(socketVhId),
                    TeamId = teamId,
                    Owner = socketNode.Node.TreeId.VhId
                },
                data: nodes,
                initializer: (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
                {
                    initializer.Init<Coupling>(new Coupling(
                        socketId: new SocketId(
                            nodeId: entities.GetId(socketVhId.NodeVhId),
                            index: socketVhId.Index))
                        );

                    initializerDelegate?.Invoke(entities, ref initializer, in id);
                });

            return nodeId;
        }
    }
}
