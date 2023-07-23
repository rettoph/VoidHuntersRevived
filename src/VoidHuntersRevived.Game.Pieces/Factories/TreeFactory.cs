﻿using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Events;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Descriptors;
using VoidHuntersRevived.Common.Pieces.Factories;
using VoidHuntersRevived.Common.Simulations.Engines;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VoidHuntersRevived.Game.Pieces.Factories
{
    internal sealed class TreeFactory : ITreeFactory, IEngine
    {
        private readonly IEntityService _entities;

        public TreeFactory(IEntityService entities)
        {
            _entities = entities;
        }

        public EntityId Create(VhId vhid, IEntityType<TreeDescriptor> tree, IEntityType<PieceDescriptor> head)
        {
            return _entities.Spawn(tree, vhid, (IEntityService entities, ref EntityInitializer initializer) =>
            {
                EntityId headId = _entities.Spawn(head, vhid.Create(1), (IEntityService entities, ref EntityInitializer initializer) =>
                {
                    initializer.Init(new Node(vhid));
                });

                initializer.Init(new Tree(headId));
            });
        }

        public EntityId Create(VhId vhid, IEntityType<TreeDescriptor> tree, EntityData nodes, EntityInitializerDelegate initializerDelegate)
        {
            return _entities.Spawn(tree, vhid, (IEntityService entities, ref EntityInitializer initializer) =>
            {
                EntityId headId = entities.Deserialize(vhid, nodes, false);

                initializer.Init<Tree>(new Tree(headId));
                initializerDelegate(entities, ref initializer);
            });
        }
    }
}
