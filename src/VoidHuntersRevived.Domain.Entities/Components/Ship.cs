using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Domain.Entities.Components
{
    public class Ship
    {
        public readonly Entity Bridge;
        public readonly AetherTree Tree;

        public Ship(Entity bridge, AetherTree tree)
        {
            this.Bridge = bridge;
            this.Tree = tree;

            this.Tree.Attach(this.Bridge);
        }
    }
}
