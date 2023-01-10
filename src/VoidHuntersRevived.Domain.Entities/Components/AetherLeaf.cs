using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Domain.Entities.Components
{
    public class AetherLeaf
    {
        public readonly Entity Entity;
        public readonly AetherTree Tree;

        internal AetherLeaf(Entity entity, AetherTree tree)
        {
            this.Entity = entity;
            this.Tree = tree;
        }
    }
}
