using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Services
{
    internal partial class SocketService : ISocketService
    {
        public EntityId Spawn(VhId sourceId, NodeSocket nodeSocket, VhId nodeVhId, Key<IEntityType> nodeTypeKey, EntityInitializerDelegate? initializerDelegate = null)
        {
            BelongsTo<Team, TeamMember> belongsToTeam = _entityQueryService.QueryById<BelongsTo<Team, TeamMember>>(nodeSocket.Node.TreeId);
            SocketVhId socketVhId = nodeSocket.Id.VhId;
            VhId treeId = nodeSocket.Node.TreeId.VhId;

            return _entitySpawnService.Spawn(sourceId, nodeTypeKey, nodeVhId, (IEntityService entities, IEntityType entityType, EntityId id, ref EntityInitializer initializer) =>
            {
                initializer.Init(belongsToTeam);
                initializer.Init(new Node(id, entities.Query.GetId(treeId)));
                initializer.Init<Coupling>(new Coupling(
                    socketId: new NodeSocketId(
                        nodeId: entities.Query.GetId(socketVhId.NodeVhId),
                        index: socketVhId.Index))
                );

                initializerDelegate?.Invoke(entities, entityType, id, ref initializer);
            });
        }

        public EntityId Spawn(VhId sourceId, NodeSocket nodeSocket, EntityData nodes, EntityInitializerDelegate? initializerDelegate = null)
        {
            BelongsTo<Team, TeamMember> belongsToTeam = _entityQueryService.QueryById<BelongsTo<Team, TeamMember>>(nodeSocket.Node.TreeId);
            SocketVhId socketVhId = nodeSocket.Id.VhId;

            EntityId nodeId = _entitySerializationService.Deserialize(
                sourceId: sourceId,
                options: new DeserializationOptions
                {
                    Seed = HashBuilder<SocketService, SocketVhId>.Instance.Calculate(socketVhId),
                    Owner = nodeSocket.Node.TreeId.VhId
                },
                data: nodes,
                initializer: (IEntityService entities, IEntityType entityType, EntityId id, ref EntityInitializer initializer) =>
                {
                    initializer.Init(belongsToTeam);
                },
                rootInitializer: (IEntityService entities, IEntityType entityType, EntityId id, ref EntityInitializer initializer) =>
                {
                    initializer.Init<Coupling>(new Coupling(
                        socketId: new NodeSocketId(
                            nodeId: entities.Query.GetId(socketVhId.NodeVhId),
                            index: socketVhId.Index))
                        );

                    initializerDelegate?.Invoke(entities, entityType, id, ref initializer);
                });

            return nodeId;
        }
    }
}
