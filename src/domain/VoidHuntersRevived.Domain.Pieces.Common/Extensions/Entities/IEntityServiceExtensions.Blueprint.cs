using Svelto.ECS;
using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Core.Utilities;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Components.Instance;

namespace VoidHuntersRevived.Common.Pieces.Extensions.Entities
{
    public static class IEntityServiceExtensions
    {
        public static EntityId Spawn(this IEntityService entities, VhId sourceId, VhId treeId, Id<ITeam> teamId, Blueprint blueprint)
        {
            VhId vhid = HashBuilder<Blueprint, VhId, Id<Blueprint>>.Instance.Calculate(treeId, blueprint.Id);

            return entities.Spawn(sourceId, treeId, teamId, vhid, blueprint.Head, default);
        }

        private static EntityId Spawn(this IEntityService entities, VhId sourceId, VhId treeId, Id<ITeam> teamId, VhId vhid, IBlueprintPiece blueprintPiece, SocketVhId socketVhId)
        {
            return entities.Spawn(sourceId, blueprintPiece.PieceType.EntityType, vhid, teamId, (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
            {
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
                            teamId,
                            childVhId,
                            blueprintPiece.Children[i][j],
                            new SocketVhId(id.VhId, (byte)i));
                    }
                }
            });
        }
    }
}
