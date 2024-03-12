using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Teams;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Descriptors;
using VoidHuntersRevived.Domain.Pieces.Common.Extensions.Entities;

namespace VoidHuntersRevived.Domain.Pieces.Services
{
    internal partial class TreeService
    {
        public EntityId Spawn(VhId sourceId, VhId vhid, Id<ITeam> teamId, IEntityType<TreeDescriptor> tree, IEntityType<PieceDescriptor> head, EntityInitializerDelegate? initializerDelegate = null)
        {
            return _entities.Spawn(sourceId, tree, vhid, (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
            {
                EntityId headId = entities.Spawn(sourceId, head, vhid.Create(1), (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
                {
                    initializer.Init(teamId);
                    initializer.Init(new Node(id, entities.GetId(vhid)));
                });

                initializer.Init(teamId);
                initializer.Init(new Tree(headId));
                initializerDelegate?.Invoke(entities, ref initializer, in id);
            });
        }

        public EntityId Spawn(VhId sourceId, VhId vhid, Id<ITeam> teamId, IEntityType<TreeDescriptor> tree, EntityData nodes, EntityInitializerDelegate initializerDelegate)
        {
            return _entities.Spawn(sourceId, tree, vhid, (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
            {
                EntityId headId = entities.Deserialize(
                    sourceId: sourceId,
                    options: new DeserializationOptions
                    {
                        Seed = HashBuilder<TreeService, VhId, byte>.Instance.Calculate(vhid, 1),
                        Owner = vhid
                    },
                    data: nodes,
                    initializer: teamId.EntityInitializer,
                    rootInitializer: (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
                    {
                        throw new Exception(); // Do we need to init this component?
                        // If not we can remove this initializer entirely.
                        initializer.Init<Coupling>(new Coupling());
                    });

                initializer.Init(teamId);
                initializer.Init<Tree>(new Tree(headId));
                initializerDelegate(entities, ref initializer, in id);
            });
        }

        public EntityId Spawn(VhId sourceId, VhId vhid, Id<ITeam> teamId, IEntityType<TreeDescriptor> tree, Blueprint blueprint, EntityInitializerDelegate? initializerDelegate = null)
        {
            return _entities.Spawn(sourceId, tree, vhid, (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
            {
                EntityId headId = entities.Spawn(sourceId, vhid, teamId, blueprint);

                initializer.Init(teamId);
                initializer.Init(new Tree(headId));
                initializerDelegate?.Invoke(entities, ref initializer, in id);
            });
        }
    }
}
