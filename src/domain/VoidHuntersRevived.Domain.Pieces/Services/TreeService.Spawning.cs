using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Extensions.Entities;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Services
{
    public partial class TreeService
    {
        public EntityId Spawn(VhId sourceId, EntityGlobalId globalId, Team team, Key<IEntityTemplate> treeTemplateKey, Key<IEntityTemplate> headNodeTemplateKey, EntityInitializerDelegate? initializerDelegate = null)
        {
            return _entitySpawnService.Spawn(sourceId, treeTemplateKey, globalId, (IEntityService entities, in InitializingEntity entity) =>
            {
                EntityId headId = entities.Spawn.Spawn(sourceId, headNodeTemplateKey, globalId.Value.Create(1).ToGlobalEntityId(), (IEntityService entities, in InitializingEntity entity) =>
                {
                    entity.Initializer.Init(team.TeamMemberComponent);
                    entity.Initializer.Init(new Node(entity.EntityId, entities.Query.GetId(globalId.Value)));
                });

                entity.Initializer.Init(team.TeamMemberComponent);
                entity.Initializer.Init(new Tree(entity.LocalId, headId.ToLocalEntityId()));
                initializerDelegate?.Invoke(entities, in entity);
            });
        }

        public EntityId Spawn(VhId sourceId, EntityGlobalId globalId, Team team, Key<IEntityTemplate> treeTemplateKey, Entities.Common.Serialization.EntityData nodes, EntityInitializerDelegate initializerDelegate)
        {
            return _entitySpawnService.Spawn(sourceId, treeTemplateKey, globalId, (IEntityService entities, in InitializingEntity tree) =>
            {
                EntityId headId = entities.Serialization.Deserialize(
                    sourceId: sourceId,
                    options: new DeserializationOptions
                    {
                        Seed = HashBuilder<TreeService, EntityGlobalId, byte>.Instance.Calculate(globalId, 1),
                        Owner = globalId.Value
                    },
                    data: nodes,
                    initializer: (IEntityService _, in InitializingEntity head) =>
                    {
                        head.Initializer.Init(team.TeamMemberComponent);
                    });

                tree.Initializer.Init(team.TeamMemberComponent);
                tree.Initializer.Init<Tree>(new Tree(tree.LocalId, headId.ToLocalEntityId()));
                initializerDelegate(entities, in tree);
            });
        }

        public EntityId Spawn(VhId sourceId, EntityGlobalId globalId, Team team, Key<IEntityTemplate> treeTemplateKey, Blueprint blueprint, EntityInitializerDelegate? initializerDelegate = null)
        {
            return _entitySpawnService.Spawn(sourceId, treeTemplateKey, globalId, (IEntityService entities, in InitializingEntity tree) =>
            {
                EntityId headId = entities.Spawn.Spawn(sourceId, globalId, team, blueprint);

                tree.Initializer.Init(team.TeamMemberComponent);
                tree.Initializer.Init(new Tree(tree.LocalId, headId.ToLocalEntityId()));
                initializerDelegate?.Invoke(entities, in tree);
            });
        }
    }
}
