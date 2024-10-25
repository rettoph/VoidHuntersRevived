using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Services
{
    public partial class SocketService : ISocketService
    {
        public EntityId Spawn(VhId sourceId, NodeSocket targetNodeSocket, VhId vhid, Key<IEntityTemplate> nodeTemplateKey, EntityInitializerDelegate? initializerDelegate = null)
        {
            Team team = _entityQueryService.QueryById<Team>(targetNodeSocket.Node.TreeId);
            SocketVhId socketVhId = targetNodeSocket.Id.VhId;
            VhId treeId = targetNodeSocket.Node.TreeId.VhId;

            return _entitySpawnService.Spawn(sourceId, nodeTemplateKey, vhid, (IEntityService entities, IEntityTemplate entityTemplate, EntityId id, ref EntityInitializer initializer) =>
            {
                initializer.Init(team);
                initializer.Init(new Node(id, entities.Query.GetId(treeId)));
                initializer.Init<Coupling>(new Coupling(
                    socketId: new NodeSocketId(
                        nodeId: entities.Query.GetId(socketVhId.NodeVhId),
                        index: socketVhId.Index))
                );

                initializerDelegate?.Invoke(entities, entityTemplate, id, ref initializer);
            });
        }

        public EntityId Spawn(VhId sourceId, NodeSocket nodeSocket, EntityData nodes, EntityInitializerDelegate? initializerDelegate = null)
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
                initializer: (IEntityService entities, IEntityTemplate entityTemplate, EntityId id, ref EntityInitializer initializer) =>
                {
                    initializer.Init(teamMember);
                },
                rootInitializer: (IEntityService entities, IEntityTemplate entityTemplate, EntityId id, ref EntityInitializer initializer) =>
                {
                    initializer.Init<Coupling>(new Coupling(
                        socketId: new NodeSocketId(
                            nodeId: entities.Query.GetId(socketVhId.NodeVhId),
                            index: socketVhId.Index))
                        );

                    initializerDelegate?.Invoke(entities, entityTemplate, id, ref initializer);
                });

            return nodeId;
        }
    }
}
