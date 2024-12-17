using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Common.Services
{
    public interface ITreeService
    {
        ref Node GetHead(Tree tree);
        ref Node GetHead(EntityLocalId treeLocalId);
        ref Node GetHead(GroupIndex treeGroupIndex);

        EntityLocalId Spawn(VhId sourceId, EntityGlobalId globalId, Team team, Key<IEntityTemplate> treeTemplateKey, Key<IEntityTemplate> headNodeTemplateKey, EntityInitializerDelegate? initializer = null);
        EntityLocalId Spawn(VhId sourceId, EntityGlobalId globalId, Team team, Key<IEntityTemplate> treeTemplateKey, Entities.Common.Serialization.EntityData nodes, EntityInitializerDelegate initializer);
        EntityLocalId Spawn(VhId sourceId, EntityGlobalId globalId, Team team, Key<IEntityTemplate> treeTemplateKey, Blueprint blueprint, EntityInitializerDelegate? initializer = null);
    }
}
