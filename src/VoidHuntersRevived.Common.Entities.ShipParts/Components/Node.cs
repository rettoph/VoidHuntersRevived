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
        public readonly int EntityId;
        public readonly int TreeId;

        public readonly Vector2 Center;
        public readonly Matrix LocalTransformation;
        public Matrix WorldTransformation;

        public Vector2 WorldPosition => Vector2.Transform(Vector2.Zero, this.WorldTransformation);
        public Vector2 CenterWorldPosition => Vector2.Transform(this.Center, this.WorldTransformation);

        public Node(int entityId, int treeId, Vector2 center, Matrix localTransformation)
        {
            this.EntityId = entityId;
            this.TreeId = treeId;

            this.Center = center;
            this.LocalTransformation = localTransformation;
            this.WorldTransformation = Matrix.Identity;
        }
    }
}
