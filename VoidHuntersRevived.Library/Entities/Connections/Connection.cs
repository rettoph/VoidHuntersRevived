using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Entities.Connections
{
    public class Connection
    {
        public Vector2 LocalPoint { get; protected set; }
        public Single LocalRotation { get; protected set; }

        public Connection(Vector2 localPoint, Single localRotation)
        {
            this.LocalPoint = localPoint;
            this.LocalRotation = localRotation;
        }
    }
}
