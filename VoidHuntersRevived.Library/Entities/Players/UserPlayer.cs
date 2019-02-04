using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.Drivers;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Networking.Interfaces;

namespace VoidHuntersRevived.Library.Entities.Players
{
    /// <summary>
    /// UserPlayer is a specific implementation of the 
    /// </summary>
    public class UserPlayer : Player
    {
        #region Private Fields
        private Driver _driver;
        #endregion

        #region Public Attributes
        public IUser User { get; private set; }
        #endregion

        #region Constructors
        public UserPlayer(IUser user, ShipPart bridge, EntityInfo info, IGame game) : base(bridge, info, game)
        {
            this.User = user;
        }
        public UserPlayer(long id, EntityInfo info, IGame game) : base(id, info, game)
        {
        }
        #endregion

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            // Create the UserPlayer's IDriver..
            _driver = this.Scene.Entities.Create<Driver>("entity:driver:user_player", null, this);
        }
        #endregion

        #region Frame Methods
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _driver.Update(gameTime);
        }
        #endregion

        #region INetworkEntity Implementation
        public override void Read(NetIncomingMessage im)
        {
            base.Read(im);

            if (im.ReadBoolean()) // Only read to the driver if the confirmation byte was recieved
                _driver.Read(im);
        }

        public override void Write(NetOutgoingMessage om)
        {
            base.Write(om);

            if (_driver == null) // No driver to send, so so data to send
                om.Write(false);
            else
            { // Send the driver confirmation byte, then send the driver data
                om.Write(true);
                _driver.Write(om);
            }
        }

        public override void FullRead(NetIncomingMessage im)
        {
            base.FullRead(im);

            // Update the UserPlayer's User value
            this.User = this.GameScene.Group.Users.GetById(im.ReadInt64());
        }

        public override void FullWrite(NetOutgoingMessage om)
        {
            base.FullWrite(om);

            // Write the players id
            om.Write(this.User.Id);
        }
        #endregion
    }
}
