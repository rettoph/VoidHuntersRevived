using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Domain.Entities.Extensions;

namespace VoidHuntersRevived.Domain.Entities.Components
{
    public class ShipPartTree
    {
        private List<ShipPartLeaf> _leaves;

        public readonly Entity Entity;
        public readonly ReadOnlyCollection<ShipPartLeaf> Leaves;

        public ShipPartTree(Entity entity)
        {
            _leaves = new List<ShipPartLeaf>();

            this.Entity = entity;
            this.Leaves = new ReadOnlyCollection<ShipPartLeaf>(_leaves);
        }

        public ShipPartLeaf Add(Entity entity)
        {
            if (!entity.IsShipPart())
            {
                throw new ArgumentException($"{nameof(ShipPartTree)}::{nameof(Add)} - Argument '{nameof(entity)}' failed {nameof(Extensions.EntityExtensions.IsShipPart)} check.");
            }

            if (entity.Has<ShipPartLeaf>())
            {
                var oldLeaf = entity.Get<ShipPartLeaf>();

                oldLeaf.Tree.Remove(oldLeaf);
            }

            var leaf = new ShipPartLeaf(entity, this);
            entity.Attach(leaf);
            _leaves.Add(leaf);

            return leaf;
        }

        public bool Remove(ShipPartLeaf leaf)
        {
            if(!_leaves.Remove(leaf))
            {
                return false;
            }

            leaf.Entity.Detach<ShipPartLeaf>();
            return true;
        }
    }
}
