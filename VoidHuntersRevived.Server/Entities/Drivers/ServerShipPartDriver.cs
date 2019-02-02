using System;
using System.Collections.Generic;
using System.Text;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.ShipParts;
using Lidgren.Network.Xna;
using VoidHuntersRevived.Library.Entities.Drivers;

namespace VoidHuntersRevived.Server.Entities.Drivers
{
    /// <summary>
    /// The server specific implementation of the IDriver, designed
    /// specifically for ShipPart instances.
    /// </summary>
    public class ServerShipPartDriver : Driver
    {
        #region Private Fields
        // The parent ShipPart
        private ShipPart _parent;
        #endregion

        #region Constructors
        public ServerShipPartDriver(ShipPart parent, EntityInfo info, IGame game) : base(info, game)
        {
            _parent = parent;
        }
        #endregion

        #region Frame Methods
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // If the current entity is awake, then it is dirty.
            // That way, all connected clients will recieve an update
            if (_parent.Body.Awake && !_parent.Dirty)
                _parent.Dirty = true;
        }
        #endregion

        #region Networking Methods (IDriver Implementation)
        public override void Read(NetIncomingMessage im)
        { // The server should never read ship part data from the client
            throw new NotImplementedException();
        }

        public override void Write(NetOutgoingMessage om)
        {
            // Write the parent ShipPart's positional data
            om.Write(_parent.Body.Position);
            om.Write(_parent.Body.LinearVelocity);
            om.Write(_parent.Body.Rotation);
            om.Write(_parent.Body.AngularVelocity);
        }
        #endregion
    }
}
