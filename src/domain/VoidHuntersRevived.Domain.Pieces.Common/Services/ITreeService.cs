using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Common.Services
{
    public interface ITreeService
    {
        ref Node GetHead(in Tree tree);
        ref Node GetHead(in EntityId treeId);
        ref Node GetHead(in GroupIndex treeGroupIndex);

        EntityId Spawn(VhId sourceId, EntityGlobalId globalId, Team team, Key<IEntityTemplate> treeTemplateKey, Key<IEntityTemplate> headNodeTemplateKey, EntityInitializerDelegate? initializer = null);
        EntityId Spawn(VhId sourceId, EntityGlobalId globalId, Team team, Key<IEntityTemplate> treeTemplateKey, Entities.Common.Serialization.EntityData nodes, EntityInitializerDelegate initializer);
        EntityId Spawn(VhId sourceId, EntityGlobalId globalId, Team team, Key<IEntityTemplate> treeTemplateKey, Blueprint blueprint, EntityInitializerDelegate? initializer = null);
    }
}
