using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Connections;

namespace VoidHuntersRevived.Library.Entities.MetaData
{
    public class HullData : ShipPartData
    {
        public readonly Vertices Vertices;
        public readonly FemaleConnection[] FemaleConnections;

        public HullData(MaleConnection maleConnection, Vector2[] vertices, FemaleConnection[] femaleConnections) : base(maleConnection)
        {
            this.Vertices = new Vertices(vertices);
            this.FemaleConnections = femaleConnections;
        }
    }
}
