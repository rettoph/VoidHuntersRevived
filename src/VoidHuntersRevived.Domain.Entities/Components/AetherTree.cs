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
    public class AetherTree
    {
        private List<AetherLeaf> _leaves;

        public readonly Entity Entity;
        public readonly ReadOnlyCollection<AetherLeaf> Leaves;

        public AetherTree(Entity entity)
        {
            _leaves = new List<AetherLeaf>();

            this.Entity = entity;
            this.Leaves = new ReadOnlyCollection<AetherLeaf>(_leaves);
        }

        public AetherLeaf Attach(Entity entity)
        {
            if(entity.Has<AetherLeaf>())
            {
                var oldLeaf = entity.Get<AetherLeaf>();

                oldLeaf.Tree.Detach(oldLeaf);
            }

            var leaf = new AetherLeaf(entity, this);
            entity.Attach(leaf);
            _leaves.Add(leaf);

            return leaf;
        }

        public bool Detach(AetherLeaf leaf)
        {
            if(!_leaves.Remove(leaf))
            {
                return false;
            }

            leaf.Entity.Detach<AetherLeaf>();
            return true;
        }
    }
}
