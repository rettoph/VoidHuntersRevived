using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.Drivers;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Server.Entities.Drivers
{
    class ServerUserPlayerDriver : Driver
    {
        #region Protected Fields
        protected UserPlayer UserPlayer;
        #endregion

        #region Constructors
        public ServerUserPlayerDriver(UserPlayer userPlayer, EntityInfo info, IGame game) : base(info, game)
        {
            this.UserPlayer = userPlayer;
        }
        #endregion

        #region Frame Methods
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // UserPlayers should be synced every frame..
            if (!this.UserPlayer.Dirty)
                this.UserPlayer.Dirty = true;
        }
        #endregion

        #region Networking Methods (Driver Implementation)
        public override void Read(NetIncomingMessage im)
        {
            if (im.SenderConnection.RemoteUniqueIdentifier == this.UserPlayer.User.Id)
            { // Only read the data if the sender connection id matches the user id
                if (im.ReadBoolean()) // Check if movement data was recieved
                    for (Int32 i = 0; i < this.UserPlayer.Movement.Count; i++) // Read the current movement settings
                        this.UserPlayer.Movement[(MovementType)im.ReadByte()] = im.ReadBoolean();
            }
            else
            { // Someone tried spoofing another players ship...
                im.SenderConnection.Disconnect("Invalid message.");
            }
        }

        public override void Write(NetOutgoingMessage om)
        {
            // Write the current UserPlayer's IUser
            om.Write(this.UserPlayer.User.Id);
        }
        #endregion
    }
}
