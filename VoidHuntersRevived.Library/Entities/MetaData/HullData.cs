using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Entities.MetaData
{
    public class HullData : ShipPartData
    {
        public readonly Vector3[] FemaleConnections;

        public HullData(Vector3 maleConnection, Vector2[] vertices, Vector3[] femaleConnections) : base(maleConnection, vertices)
        {
            this.FemaleConnections = femaleConnections;
        }
    }
}
