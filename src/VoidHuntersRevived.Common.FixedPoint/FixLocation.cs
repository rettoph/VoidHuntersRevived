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
    public readonly struct FixLocation
    {
        public readonly FixMatrix Transformation;
        public readonly FixVector2 Position;
        public readonly Fix64 Rotation;

        public FixLocation(FixVector2 position, Fix64 rotation)
        {
            this.Transformation = FixMatrix.CreateRotationZ(rotation) * FixMatrix.CreateTranslation(position.X, position.Y, Fix64.Zero);
            this.Position = position;
            this.Rotation = rotation;
        }
    }
}
