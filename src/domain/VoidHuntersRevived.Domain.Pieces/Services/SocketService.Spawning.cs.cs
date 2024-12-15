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
            TeamMember teamMember = _entityQueryService.QueryByLocalId<TeamMember>(targetNodeSocket.Node.TreeLocalId);
            SocketVhId socketVhId = targetNodeSocket.Id.VhId;
            EntityLocalId treeLocalId = targetNodeSocket.Node.TreeLocalId;

            return _entitySpawnService.Spawn(sourceId, nodeTemplateKey, globalId, (IEntityService entities, in InitializingEntity entity) =>
            {
                entity.Initializer.Init(teamMember);
                entity.Initializer.Init(new Node(entity.EntityId, treeLocalId));
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
            TeamMember teamMember = _entityQueryService.QueryByLocalId<TeamMember>(nodeSocket.Node.TreeLocalId);
            SocketVhId socketVhId = nodeSocket.Id.VhId;
            EntityGlobalId treeGlobalId = _entityQueryService.GetGlobalId(nodeSocket.Node.TreeLocalId);

            EntityId nodeId = _entitySerializationService.Deserialize(
                sourceId: sourceId,
                options: new DeserializationOptions
                {
                    Seed = HashBuilder<SocketService, VhId, SocketVhId>.Instance.Calculate(sourceId, socketVhId),
                    Owner = treeGlobalId.Value
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
