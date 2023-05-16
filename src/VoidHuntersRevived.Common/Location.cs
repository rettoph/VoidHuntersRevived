using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common
{
    public readonly struct Location
    {
        public readonly Matrix Transformation;
        public readonly Vector2 Position;
        public readonly float Rotation;

        public Location(Vector2 position, float rotation)
        {
            this.Transformation = Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(position.X, position.Y, 0);
            this.Position = position;
            this.Rotation = rotation;
        }
    }
}
