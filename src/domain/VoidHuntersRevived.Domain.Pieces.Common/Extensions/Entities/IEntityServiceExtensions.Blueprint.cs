using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Common.Extensions.Entities
{
    public static class IEntityServiceExtensions
    {
        public static EntityId Spawn(this IEntityService entities, VhId sourceId, VhId treeId, BelongsTo<Team, TeamMember> belongsToTeam, Blueprint blueprint)
        {
            VhId vhid = HashBuilder<Blueprint, VhId, Id<Blueprint>>.Instance.Calculate(treeId, blueprint.Id);

            return entities.Spawn(sourceId, treeId, belongsToTeam, vhid, blueprint.Head, default);
        }

        private static EntityId Spawn(this IEntityService entities, VhId sourceId, VhId treeId, BelongsTo<Team, TeamMember> belongsToTeam, VhId vhid, IBlueprintPiece blueprintPiece, SocketVhId socketVhId)
        {
            return entities.Spawn(sourceId, blueprintPiece.PieceType, vhid, (IEntityService entities, IEntityType type, in EntityId id, ref EntityInitializer initializer) =>
            {
                initializer.Init(belongsToTeam);
                initializer.Init(new Node(id, entities.GetId(treeId)));

                if (socketVhId != default)
                {
                    initializer.Init<Coupling>(new Coupling(
                        socketId: new SocketId(
                            nodeId: entities.GetId(socketVhId.NodeVhId),
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

                        entities.Spawn(
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
