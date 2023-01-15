using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Configurations
{
    public sealed class JointConfiguration
    {
        public readonly Vector2 Position;
        public readonly float Rotation;

        public readonly Matrix Transformation;

        public JointConfiguration(Vector2 position, float rotation)
        {
            this.Position = position;
            this.Rotation = rotation;

            this.Transformation = Matrix.CreateRotationZ(this.Rotation) * Matrix.CreateTranslation(this.Position.X, this.Position.Y, 0);
        }
    }
}
