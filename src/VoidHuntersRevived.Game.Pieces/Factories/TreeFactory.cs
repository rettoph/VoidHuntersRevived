using Svelto.ECS;
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
            VhId headId = vhid.Create(1);
            _entities.Spawn(head, headId, (IEntityService entities, ref EntityInitializer initializer) =>
            {
                initializer.Init(new Node(vhid));
            });

            return _entities.Spawn(tree, vhid, (IEntityService entities, ref EntityInitializer initializer) =>
            {
                initializer.Init(new Tree(headId));
            });
        }

        public EntityId Create(VhId vhid, IEntityType<TreeDescriptor> tree, EntityData nodes, EntityInitializerDelegate initializerDelegate)
        {
            EntityId headId = _entities.Deserialize(vhid, nodes, false);

            return _entities.Spawn(tree, vhid, (IEntityService entities, ref EntityInitializer initializer) =>
            {
                initializer.Init<Tree>(new Tree(headId.VhId));
                initializerDelegate(entities, ref initializer);
            });
        }
    }
}
