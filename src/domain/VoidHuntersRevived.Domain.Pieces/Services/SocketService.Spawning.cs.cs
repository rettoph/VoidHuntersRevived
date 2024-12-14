using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Services
{
    public partial class SocketService : ISocketService
    {
        public EntityId Spawn(VhId sourceId, NodeSocket targetNodeSocket, EntityGlobalId globalId, Key<IEntityTemplate> nodeTemplateKey, EntityInitializerDelegate? initializerDelegate = null)
        {
            TeamMember teamMember = _entityQueryService.QueryById<TeamMember>(targetNodeSocket.Node.TreeId);
            SocketVhId socketVhId = targetNodeSocket.Id.VhId;
            VhId treeId = targetNodeSocket.Node.TreeId.VhId;

            return _entitySpawnService.Spawn(sourceId, nodeTemplateKey, globalId, (IEntityService entities, in InitializingEntity entity) =>
            {
                entity.Initializer.Init(teamMember);
                entity.Initializer.Init(new Node(entity.EntityId, entities.Query.GetId(treeId)));
                entity.Initializer.Init<Coupling>(new Coupling(
                    socketId: new NodeSocketId(
                        nodeId: entities.Query.GetId(socketVhId.NodeVhId),
                        index: socketVhId.Index))
                );

                initializerDelegate?.Invoke(entities, in entity);
            });
        }

        public EntityId Spawn(VhId sourceId, NodeSocket nodeSocket, Entities.Common.Serialization.EntityData nodes, EntityInitializerDelegate? initializerDelegate = null)
        {
            TeamMember teamMember = _entityQueryService.QueryById<TeamMember>(nodeSocket.Node.TreeId);
            SocketVhId socketVhId = nodeSocket.Id.VhId;

            EntityId nodeId = _entitySerializationService.Deserialize(
                sourceId: sourceId,
                options: new DeserializationOptions
                {
                    Seed = HashBuilder<SocketService, VhId, SocketVhId>.Instance.Calculate(sourceId, socketVhId),
                    Owner = nodeSocket.Node.TreeId.VhId
                },
                data: nodes,
                initializer: (IEntityService entities, in InitializingEntity entity) =>
                {
                    entity.Initializer.Init(teamMember);
                },
                rootInitializer: (IEntityService entities, in InitializingEntity entity) =>
                {
                    entity.Initializer.Init<Coupling>(new Coupling(
                        socketId: new NodeSocketId(
                            nodeId: entities.Query.GetId(socketVhId.NodeVhId),
                            index: socketVhId.Index))
                        );

                    initializerDelegate?.Invoke(entities, in entity);
                });

            return nodeId;
        }
    }
}
