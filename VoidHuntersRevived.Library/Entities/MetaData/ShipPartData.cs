using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Entities.MetaData
{
    public class ShipPartData
    {
        public readonly Vector3 MaleConnection;
        public readonly Vector2[] Vertices;
        public readonly Vector3[] FemaleConnections;

        public ShipPartData(Vector3 maleConnection, Vector2[] vertices, Vector3[] femaleConnections)
        {
            this.MaleConnection = maleConnection;
            this.Vertices = vertices;
            this.FemaleConnections = femaleConnections ?? new Vector3[0];
        }
    }
}
