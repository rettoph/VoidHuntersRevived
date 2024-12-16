using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Common.Extensions.Entities
{
    public static class IEntityServiceExtensions
    {
        public static EntityId Spawn(this IEntitySpawnService entitySpawnService, VhId sourceId, EntityGlobalId treeId, Team team, Blueprint blueprint)
        {
            EntityGlobalId globalId = HashBuilder<Blueprint, EntityGlobalId, Id<Blueprint>>.Instance.Calculate(treeId, blueprint.Id).ToGlobalEntityId();

            return entitySpawnService.Spawn(sourceId, treeId, team, globalId, blueprint.Head, default);
        }

        private static EntityId Spawn(this IEntitySpawnService entitySpawnService, VhId sourceId, EntityGlobalId treeId, Team team, EntityGlobalId globalId, IBlueprintPiece blueprintPiece, NodeSocketGlobalId socketVhId)
        {
            return entitySpawnService.Spawn(sourceId, blueprintPiece.PieceTemplateKey, globalId, (IEntityService entities, in InitializingEntity entity) =>
            {
                entity.Initializer.Init(team.TeamMemberComponent);
                entity.Initializer.Init(new Node(entity.LocalId, entities.Query.GetLocalId(treeId)));

                if (socketVhId != default)
                {
                    entity.Initializer.Init<Coupling>(new Coupling(
                        socketId: new NodeSocketLocalId(
                            nodeLocalId: entities.Query.GetLocalId(socketVhId.NodeGlobalId),
                            socketIndex: socketVhId.SocketIndex))
                    );
                }

                for (int i = 0; i < blueprintPiece.Children.Length; i++)
                {
                    for (int j = 0; j < blueprintPiece.Children[i].Length; j++)
                    {
                        EntityGlobalId childGlobalId = HashBuilder<Blueprint, EntityGlobalId, VhId, int, int>.Instance.Calculate(
                            treeId,
                            entity.EntityId.VhId,
                            i,
                            j).ToGlobalEntityId();

                        entities.Spawn.Spawn(
                            sourceId,
                            treeId,
                            team,
                            childGlobalId,
                            blueprintPiece.Children[i][j],
                            new NodeSocketGlobalId(entity.EntityId.ToGlobalEntityId(), (byte)i));
                    }
                }
            });
        }
    }
}
