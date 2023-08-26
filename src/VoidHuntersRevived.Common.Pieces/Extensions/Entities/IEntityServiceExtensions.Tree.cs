﻿using System;
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

namespace VoidHuntersRevived.Common.Pieces.Extensions.Entities
{
    public static class IEntityServiceExtensions
    {
        public static void SpawnTree(this IEntityService entities, VhId vhid, IEntityType<TreeDescriptor> tree, IEntityType<PieceDescriptor> head)
        {
            entities.Spawn(tree, vhid, (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
            {
                VhId headVhId = vhid.Create(1);

                entities.Spawn(head, headVhId, (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
                {
                    initializer.Init(new Node(id, entities.GetId(vhid)));
                });

                initializer.Init(new Tree(entities.GetId(headVhId)));
            });
        }

        public static void SpawnTree(this IEntityService entities, VhId vhid, IEntityType<TreeDescriptor> tree, EntityData nodes, EntityInitializerDelegate initializer)
        {
            entities.Spawn(tree, vhid, (IEntityService entities, ref EntityInitializer init, in EntityId id) =>
            {
                EntityId headId = entities.Deserialize(
                    seed: vhid,
                    data: nodes,
                    initializer: (IEntityService entities, ref EntityInitializer init, in EntityId id) =>
                    {
                        init.Init<Coupling>(new Coupling());
                    },
                    confirmed: false);

                init.Init<Tree>(new Tree(headId));
                initializer(entities, ref init, in id);
            });
        }
    }
}
