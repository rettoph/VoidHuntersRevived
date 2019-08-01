using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Configurations
{
    public class ConnectionNodeConfiguration
    {
        public Vector2 Position;
        public Single Rotation;

        public static ConnectionNodeConfiguration Transform(ConnectionNodeConfiguration node, Matrix transformation)
        {
            var point = Vector2.Transform(node.Position, transformation);
            // Create a new vector 2 that represents the node's rotation target...
            var rotationPoint = node.Position + Vector2.Transform(Vector2.UnitX, Matrix.CreateRotationZ(node.Rotation));
            // Transform the rotation reference point...
            rotationPoint = Vector2.Transform(rotationPoint, transformation);
            // Convert the new rotation point back into a rotation...
            var rotation = RadianHelper.Normalize((Single)Math.Atan2(rotationPoint.Y - point.Y, rotationPoint.X - point.X));

            return new ConnectionNodeConfiguration()
            {
                Position = point,
                Rotation = rotation
            };
        }
    }
}
