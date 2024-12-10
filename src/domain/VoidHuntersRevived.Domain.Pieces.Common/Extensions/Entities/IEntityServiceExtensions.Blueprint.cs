using Svelto.ECS;
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
        public static EntityId Spawn(this IEntitySpawnService entitySpawnService, VhId sourceId, GlobalEntityId treeId, Team team, Blueprint blueprint)
        {
            GlobalEntityId globalId = HashBuilder<Blueprint, GlobalEntityId, Id<Blueprint>>.Instance.Calculate(treeId, blueprint.Id).ToGlobalEntityId();

            return entitySpawnService.Spawn(sourceId, treeId, team, globalId, blueprint.Head, default);
        }

        private static EntityId Spawn(this IEntitySpawnService entitySpawnService, VhId sourceId, GlobalEntityId treeId, Team team, GlobalEntityId globalId, IBlueprintPiece blueprintPiece, SocketVhId socketVhId)
        {
            return entitySpawnService.Spawn(sourceId, blueprintPiece.PieceTemplateKey, globalId, (IEntityService entities, IEntityTemplate entityTemplate, EntityId id, ref EntityInitializer initializer) =>
            {
                initializer.Init(team.TeamMemberComponent);
                initializer.Init(new Node(id, entities.Query.GetId(treeId.Value)));

                if (socketVhId != default)
                {
                    initializer.Init<Coupling>(new Coupling(
                        socketId: new NodeSocketId(
                            nodeId: entities.Query.GetId(socketVhId.NodeVhId),
                            index: socketVhId.Index))
                    );
                }

                for (int i = 0; i < blueprintPiece.Children.Length; i++)
                {
                    for (int j = 0; j < blueprintPiece.Children[i].Length; j++)
                    {
                        GlobalEntityId childGlobalId = HashBuilder<Blueprint, GlobalEntityId, VhId, int, int>.Instance.Calculate(
                            treeId,
                            id.VhId,
                            i,
                            j).ToGlobalEntityId();

                        entities.Spawn.Spawn(
                            sourceId,
                            treeId,
                            team,
                            childGlobalId,
                            blueprintPiece.Children[i][j],
                            new SocketVhId(id.VhId, (byte)i));
                    }
                }
            });
        }
    }
}
