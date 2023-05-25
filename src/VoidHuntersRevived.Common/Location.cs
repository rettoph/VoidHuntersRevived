using FixedMath.NET;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Common;

namespace VoidHuntersRevived.Common
{
    public readonly struct Location
    {
        public readonly FixedMatrix Transformation;
        public readonly AetherVector2 Position;
        public readonly Fix64 Rotation;

        public Location(AetherVector2 position, Fix64 rotation)
        {
            this.Transformation = FixedMatrix.CreateRotationZ(rotation) * FixedMatrix.CreateTranslation(position.X, position.Y, Fix64.Zero);
            this.Position = position;
            this.Rotation = rotation;
        }
    }
}
