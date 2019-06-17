using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Client.Library.Utilities
{
    /// <summary>
    /// A server render for the client to simulate exactly what is
    /// going on at all times. Bodies may be added with a FarseerEntity
    /// key
    /// </summary>
    public class ServerRender
    {
        public readonly World World;
        public Dictionary<FarseerEntity, Body> Bodies { get; private set; }

        public ServerRender(World world)
        {
            this.World = world;

            this.Bodies = new Dictionary<FarseerEntity, Body>();
        }
    }
}
