using FarseerPhysics.Dynamics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Client.Library.Utilities
{
    /// <summary>
    /// Represents the server version of all FarseerEntity instances.
    /// 
    /// This will create a clone of each FarseerEntity's body and can be used
    /// to manipulate the client's server data without instantly changing the
    /// rendered objects. 
    /// 
    /// This is useful for lerping the client render towards the server data
    /// smoothly. The internal server shadow data is rigid and snaps instantly
    /// while the client FarseerEntity body smoothly chases after it.
    /// </summary>
    public sealed class ServerShadow
    {
        #region Private Fields
        private Dictionary<FarseerEntity, Body> _bodies;
        #endregion

        #region Public Properties
        public World World { get; private set; }

        public Body this[FarseerEntity key] {
            get
            {
                this.World.ProcessChanges();
                return _bodies[key];
            }
        }
        #endregion

        #region Constructor
        public ServerShadow(World world)
        {
            _bodies = new Dictionary<FarseerEntity, Body>();

            this.World = new World(world.Gravity);

            this.World.BodyAdded += this.HandleBodyAdded;
            this.World.BodyRemoved += this.HandleBodyRemoved;
        }
        #endregion

        #region Event Handlers
        private void HandleBodyRemoved(Body body)
        {
            _bodies.Remove(body.UserData as FarseerEntity);
        }

        private void HandleBodyAdded(Body body)
        {
            _bodies.Add(body.UserData as FarseerEntity, body);
        }
        #endregion
    }
}
