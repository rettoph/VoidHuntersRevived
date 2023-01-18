using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;

namespace VoidHuntersRevived.Domain.Entities.Components
{
    public class Ship
    {
        public readonly Entity Bridge;
        public readonly Entity Tree;

        public Ship(Entity bridge, Entity tree)
        {
            this.Bridge = bridge;
            this.Tree = tree;
        }
    }
}
