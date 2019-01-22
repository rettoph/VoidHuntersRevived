using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Entities.MetaData
{
    public class HullData : ShipPartData
    {
        public readonly Vertices Vertices;

        public HullData(Vector2[] vertices)
        {
            this.Vertices = new Vertices(vertices);
        }
    }
}
