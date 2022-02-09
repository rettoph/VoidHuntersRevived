using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Contexts.Utilities
{
    public class ConnectionNodeContext
    {
        public Vector2 Position;
        public Single Rotation;

        public static ConnectionNodeContext Transform(ConnectionNodeContext node, Matrix transformation)
        {
            if (node == default || transformation == default)
                return node;

            var point = Vector2.Transform(node.Position, transformation);
            // Create a new vector 2 that represents the node's rotation target...
            var rotationPoint = node.Position + Vector2.Transform(Vector2.UnitX, Matrix.CreateRotationZ(node.Rotation));
            // Transform the rotation reference point...
            rotationPoint = Vector2.Transform(rotationPoint, transformation);
            // Convert the new rotation point back into a rotation...
            var rotation = MathHelper.WrapAngle((Single)Math.Atan2(rotationPoint.Y - point.Y, rotationPoint.X - point.X));

            return new ConnectionNodeContext()
            {
                Position = point,
                Rotation = rotation
            };
        }
    }
}
