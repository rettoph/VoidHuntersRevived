using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Pieces.Descriptors;
using VoidHuntersRevived.Common.Entities.Services;
using Svelto.ECS;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Entities.Options;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Common.Pieces.Extensions.Entities
{
    public static class IEntityServiceExtensions
    {
        public static EntityId Spawn(this IEntityService entities, VhId treeId, Id<ITeam> teamId, IBlueprint blueprint)
        {
            VhId vhid = HashBuilder<BlueprintPieceDto, VhId, Id<IBlueprint>>.Instance.Calculate(treeId, blueprint.Id);

            return entities.Spawn(treeId, teamId, vhid, blueprint.Head);
        }

        private static EntityId Spawn(this IEntityService entities, VhId treeId, Id<ITeam> teamId, VhId vhid, IBlueprintPiece blueprintPiece)
        {
            return entities.Spawn(blueprintPiece.Type.EntityType, vhid, teamId, (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
            {
                initializer.Init(new Node(id, entities.GetId(treeId)));
            });
        }
    }
}
