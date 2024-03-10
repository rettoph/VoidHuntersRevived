using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Descriptors;
using VoidHuntersRevived.Domain.Pieces.Common.Services;

namespace VoidHuntersRevived.Domain.Pieces.Services
{
    internal partial class SocketService : ISocketService
    {
        public EntityId Spawn(VhId sourceId, Socket socket, VhId nodeVhId, IEntityType<PieceDescriptor> node, InstanceEntityInitializerDelegate? initializerDelegate = null)
        {
            Id<ITeam> teamId = _entities.QueryById<Id<ITeam>>(socket.Node.TreeId);
            SocketVhId socketVhId = socket.Id.VhId;
            VhId treeId = socket.Node.TreeId.VhId;

            return _entities.Spawn(sourceId, node, nodeVhId, teamId, (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
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

        public EntityId Spawn(VhId sourceId, Socket socket, EntityData nodes, InstanceEntityInitializerDelegate? initializerDelegate = null)
        {
            Id<ITeam> teamId = _entities.QueryById<Id<ITeam>>(socket.Node.TreeId);
            SocketVhId socketVhId = socket.Id.VhId;

            EntityId nodeId = _entities.Deserialize(
                sourceId: sourceId,
                options: new DeserializationOptions
                {
                    Seed = HashBuilder<SocketService, SocketVhId>.Instance.Calculate(socketVhId),
                    TeamId = teamId,
                    Owner = socket.Node.TreeId.VhId
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
