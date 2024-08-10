using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Extensions.Entities;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Services
{
    internal partial class TreeService
    {
        public EntityId Spawn(VhId sourceId, VhId vhid, BelongsTo<Team, TeamMember> belongsToTeam, IKey<IEntityType> treeTypeKey, IKey<IEntityType> headNodeTypeKey, EntityInitializerDelegate? initializerDelegate = null)
        {
            return _entitySpawnService.Spawn(sourceId, treeTypeKey, vhid, (IEntityService entities, IEntityTypeProvider treeTypeProvider, EntityId id, ref EntityInitializer initializer) =>
            {
                EntityId headId = entities.Spawn.Spawn(sourceId, headNodeTypeKey, vhid.Create(1), (IEntityService entities, IEntityTypeProvider provider, EntityId id, ref EntityInitializer initializer) =>
                {
                    initializer.Init(belongsToTeam);
                    initializer.Init(new Node(id, entities.Query.GetId(vhid)));
                });

                initializer.Init(belongsToTeam);
                initializer.Init(new Tree(headId));
                initializerDelegate?.Invoke(entities, treeTypeProvider, id, ref initializer);
            });
        }

        public EntityId Spawn(VhId sourceId, VhId vhid, BelongsTo<Team, TeamMember> belongsToTeam, IKey<IEntityType> treeTypeKey, EntityData nodes, EntityInitializerDelegate initializerDelegate)
        {
            return _entitySpawnService.Spawn(sourceId, treeTypeKey, vhid, (IEntityService entities, IEntityTypeProvider treeTypeProvider, EntityId id, ref EntityInitializer initializer) =>
            {
                EntityId headId = entities.Serialization.Deserialize(
                    sourceId: sourceId,
                    options: new DeserializationOptions
                    {
                        Seed = HashBuilder<TreeService, VhId, byte>.Instance.Calculate(vhid, 1),
                        Owner = vhid
                    },
                    data: nodes,
                    initializer: (IEntityService _, IEntityTypeProvider _, EntityId _, ref EntityInitializer initializer) =>
                    {
                        initializer.Init(belongsToTeam);
                    });

                initializer.Init(belongsToTeam);
                initializer.Init<Tree>(new Tree(headId));
                initializerDelegate(entities, treeTypeProvider, id, ref initializer);
            });
        }

        public EntityId Spawn(VhId sourceId, VhId vhid, BelongsTo<Team, TeamMember> belongsToTeam, IKey<IEntityType> treeTypeKey, Blueprint blueprint, EntityInitializerDelegate? initializerDelegate = null)
        {
            return _entitySpawnService.Spawn(sourceId, treeTypeKey, vhid, (IEntityService entities, IEntityTypeProvider treeTypeProvider, EntityId id, ref EntityInitializer initializer) =>
            {
                EntityId headId = entities.Spawn.Spawn(sourceId, vhid, belongsToTeam, blueprint);

                initializer.Init(belongsToTeam);
                initializer.Init(new Tree(headId));
                initializerDelegate?.Invoke(entities, treeTypeProvider, id, ref initializer);
            });
        }
    }
}
