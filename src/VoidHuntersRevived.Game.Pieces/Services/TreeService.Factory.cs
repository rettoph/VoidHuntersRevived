using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Descriptors;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Game.Pieces.Services
{
    internal partial class TreeService
    {
        public EntityId Spawn(VhId vhid, TeamId teamId, IEntityType<TreeDescriptor> tree, IEntityType<PieceDescriptor> head)
        {
            return _entities.Spawn(tree, vhid, teamId, (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
            {
                EntityId headId = entities.Spawn(head, vhid.Create(1), teamId , (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
                {
                    initializer.Init(new Node(id, entities.GetId(vhid)));
                });

                initializer.Init(new Tree(headId));
            });
        }

        public EntityId Spawn(VhId vhid, TeamId teamId, IEntityType<TreeDescriptor> tree, EntityData nodes, EntityInitializerDelegate initializerDelegate)
        {
            return _entities.Spawn(tree, vhid, teamId, (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
            {
                EntityId headId = entities.Deserialize(
                    seed: HashBuilder<TreeService, VhId, byte>.Instance.Calculate(vhid, 1),
                    teamId: teamId,
                    injection: vhid,
                    data: nodes,
                    initializer: (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
                    {
                        initializer.Init<Coupling>(new Coupling());
                    });

                initializer.Init<Tree>(new Tree(headId));
                initializerDelegate(entities, ref initializer, in id);
            });
        }
    }
}
