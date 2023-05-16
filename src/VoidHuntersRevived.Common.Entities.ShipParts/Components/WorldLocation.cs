using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Components
{
    public abstract class WorldLocation
    {
        public abstract Vector2 Position { get; }
        public abstract float Rotation { get; }
        public abstract Matrix Transformation { get; }

        public abstract void SetTransform(Vector2 position, float rotation);
    }
}
