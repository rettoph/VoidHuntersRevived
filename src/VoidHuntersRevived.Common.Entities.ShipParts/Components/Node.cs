using Microsoft.Xna.Framework;
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

        public readonly Matrix LocalTransformation;
        public Matrix WorldTransformation;

        internal Node(Entity entity, Tree tree, Matrix localTransformation)
        {
            this.Entity = entity;
            this.Tree = tree;

            this.LocalTransformation = localTransformation;
            this.WorldTransformation = Matrix.Identity;
        }
    }
}
