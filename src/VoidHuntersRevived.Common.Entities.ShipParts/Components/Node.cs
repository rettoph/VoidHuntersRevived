using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Components
{
    public class Node
    {
        public readonly Entity Entity;
        public readonly Tree Tree;

        internal Node(Entity entity, Tree tree)
        {
            this.Entity = entity;
            this.Tree = tree;
        }
    }
}
