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
        public abstract FixVector2 Position { get; }
        public abstract Fix64 Rotation { get; }
        public abstract FixMatrix Transformation { get; }

        public abstract void SetTransform(FixVector2 position, Fix64 rotation);
    }
}
