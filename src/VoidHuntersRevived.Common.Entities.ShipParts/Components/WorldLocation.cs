using FixedMath.NET;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Common;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Components
{
    public abstract class WorldLocation
    {
        public abstract AetherVector2 Position { get; }
        public abstract Fix64 Rotation { get; }
        public abstract FixedMatrix Transformation { get; }

        public abstract void SetTransform(AetherVector2 position, Fix64 rotation);
    }
}
