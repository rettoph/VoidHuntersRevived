using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
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
            return _entitySpawnService.Spawn(sourceId, treeTemplateKey, globalId, (IEntityService entities, IEntityTemplate treeTemplate, EntityId id, ref EntityInitializer initializer) =>
            {
                EntityId headId = entities.Spawn.Spawn(sourceId, headNodeTemplateKey, globalId.Value.Create(1).ToGlobalEntityId(), (IEntityService entities, IEntityTemplate entityTemplate, EntityId id, ref EntityInitializer initializer) =>
                {
                    initializer.Init(team.TeamMemberComponent);
                    initializer.Init(new Node(id, entities.Query.GetId(globalId.Value)));
                });

                initializer.Init(team.TeamMemberComponent);
                initializer.Init(new Tree(id, headId));
                initializerDelegate?.Invoke(entities, treeTemplate, id, ref initializer);
            });
        }

        public EntityId Spawn(VhId sourceId, EntityGlobalId globalId, Team team, Key<IEntityTemplate> treeTemplateKey, EntityData nodes, EntityInitializerDelegate initializerDelegate)
        {
            return _entitySpawnService.Spawn(sourceId, treeTemplateKey, globalId, (IEntityService entities, IEntityTemplate treeTemplate, EntityId id, ref EntityInitializer initializer) =>
            {
                EntityId headId = entities.Serialization.Deserialize(
                    sourceId: sourceId,
                    options: new DeserializationOptions
                    {
                        Seed = HashBuilder<TreeService, EntityGlobalId, byte>.Instance.Calculate(globalId, 1),
                        Owner = globalId.Value
                    },
                    data: nodes,
                    initializer: (IEntityService _, IEntityTemplate _, EntityId _, ref EntityInitializer initializer) =>
                    {
                        initializer.Init(team.TeamMemberComponent);
                    });

                initializer.Init(team.TeamMemberComponent);
                initializer.Init<Tree>(new Tree(id, headId));
                initializerDelegate(entities, treeTemplate, id, ref initializer);
            });
        }

        public EntityId Spawn(VhId sourceId, EntityGlobalId globalId, Team team, Key<IEntityTemplate> treeTemplateKey, Blueprint blueprint, EntityInitializerDelegate? initializerDelegate = null)
        {
            return _entitySpawnService.Spawn(sourceId, treeTemplateKey, globalId, (IEntityService entities, IEntityTemplate treeTemplate, EntityId id, ref EntityInitializer initializer) =>
            {
                EntityId headId = entities.Spawn.Spawn(sourceId, globalId, team, blueprint);

                initializer.Init(team.TeamMemberComponent);
                initializer.Init(new Tree(id, headId));
                initializerDelegate?.Invoke(entities, treeTemplate, id, ref initializer);
            });
        }
    }
}
