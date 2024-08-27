using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Common.Extensions.Entities
{
    public static class IEntityServiceExtensions
    {
        public static EntityId Spawn(this IEntitySpawnService entitySpawnService, VhId sourceId, VhId treeId, BelongsTo<Team, TeamMember> belongsToTeam, Blueprint blueprint)
        {
            VhId vhid = HashBuilder<Blueprint, VhId, Id<Blueprint>>.Instance.Calculate(treeId, blueprint.Id);

            return entitySpawnService.Spawn(sourceId, treeId, belongsToTeam, vhid, blueprint.Head, default);
        }

        private static EntityId Spawn(this IEntitySpawnService entitySpawnService, VhId sourceId, VhId treeId, BelongsTo<Team, TeamMember> belongsToTeam, VhId vhid, IBlueprintPiece blueprintPiece, SocketVhId socketVhId)
        {
            return entitySpawnService.Spawn(sourceId, blueprintPiece.PieceTypeKey, vhid, (IEntityService entities, IEntityType entityType, EntityId id, ref EntityInitializer initializer) =>
            {
                initializer.Init(belongsToTeam);
                initializer.Init(new Node(id, entities.Query.GetId(treeId)));

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
                        VhId childVhId = HashBuilder<Blueprint, VhId, VhId, int, int>.Instance.Calculate(
                            treeId,
                            id.VhId,
                            i,
                            j);

                        entities.Spawn.Spawn(
                            sourceId,
                            treeId,
                            belongsToTeam,
                            childVhId,
                            blueprintPiece.Children[i][j],
                            new SocketVhId(id.VhId, (byte)i));
                    }
                }
            });
        }
    }
}
