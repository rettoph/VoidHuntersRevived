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
    public partial class SocketService : INodeSocketService
    {
        public EntityId Spawn(VhId sourceId, NodeSocket targetNodeSocket, EntityGlobalId globalId, Key<IEntityTemplate> nodeTemplateKey, EntityInitializerDelegate? initializerDelegate = null)
        {
            TeamMember teamMember = _entityQueryService.QueryByLocalId<TeamMember>(targetNodeSocket.Node.TreeLocalId);
            NodeSocketGlobalId targetNodeSocketGlobalId = this.GetGlobalId(targetNodeSocket.LocalId);
            EntityGlobalId treeGlobalId = _entityQueryService.GetGlobalId(targetNodeSocket.Node.TreeLocalId);

            return _entitySpawnService.Spawn(sourceId, nodeTemplateKey, globalId, (IEntityService entities, in InitializingEntity entity) =>
            {
                entity.Initializer.Init(teamMember);
                entity.Initializer.Init(new Node(entity.LocalId, entities.Query.GetLocalId(treeGlobalId)));
                entity.Initializer.Init<Coupling>(new Coupling(
                    socketId: new NodeSocketLocalId(
                        nodeLocalId: entities.Query.GetLocalId(targetNodeSocketGlobalId.NodeGlobalId),
                        socketIndex: targetNodeSocketGlobalId.SocketIndex))
                );

                initializerDelegate?.Invoke(entities, in entity);
            });
        }

        public EntityId Spawn(VhId sourceId, NodeSocket targetNodeSocket, EntityData nodes, EntityInitializerDelegate? initializerDelegate = null)
        {
            TeamMember teamMember = _entityQueryService.QueryByLocalId<TeamMember>(targetNodeSocket.Node.TreeLocalId);
            NodeSocketGlobalId targetNodeSocketGlobalId = this.GetGlobalId(targetNodeSocket.LocalId);
            EntityGlobalId treeGlobalId = _entityQueryService.GetGlobalId(targetNodeSocket.Node.TreeLocalId);

            EntityId nodeId = _entitySerializationService.Deserialize(
                sourceId: sourceId,
                options: new DeserializationOptions
                {
                    Seed = HashBuilder<SocketService, VhId, NodeSocketGlobalId>.Instance.Calculate(sourceId, targetNodeSocketGlobalId),
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
                        socketId: new NodeSocketLocalId(
                        nodeLocalId: entities.Query.GetLocalId(targetNodeSocketGlobalId.NodeGlobalId),
                        socketIndex: targetNodeSocketGlobalId.SocketIndex))
                    );

                    initializerDelegate?.Invoke(entities, in entity);
                });

            return nodeId;
        }
    }
}
