using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Domain.Entities.Components
{
    public class ShipPartLeaf
    {
        public readonly Entity Entity;
        public readonly ShipPartTree Tree;

        internal ShipPartLeaf(Entity entity, ShipPartTree tree)
        {
            this.Entity = entity;
            this.Tree = tree;
        }
    }
}
