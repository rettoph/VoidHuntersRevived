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
        public readonly Vertices Vertices;

        public ShipPartData(Vector3 maleConnection, Vector2[] vertices)
        {
            this.MaleConnection = maleConnection;
            this.Vertices = new Vertices(vertices);
        }
    }
}
