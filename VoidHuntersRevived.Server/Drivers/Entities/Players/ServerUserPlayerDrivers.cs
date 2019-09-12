using GalacticFighters.Library.Entities.Players;
using Guppy;
using Guppy.Attributes;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using static GalacticFighters.Library.Entities.Ship;

namespace GalacticFighters.Server.Drivers.Entities.Players
{
    [IsDriver(typeof(UserPlayer))]
    public class ServerUserPlayerDrivers : Driver<UserPlayer>
    {
        #region Constructor
        public ServerUserPlayerDrivers(UserPlayer driven) : base(driven)
        {
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.driven.Actions.TryAdd("direction:changed:request", this.HandleDirectionChangedRequest);
        }
        #endregion

        #region Action Handlers
        private void HandleDirectionChangedRequest(object sender, NetIncomingMessage im)
        {
            if(this.ValidateSender(im))
            { // If the message checks out... update the ships direction.
                this.driven.Ship.SetDirection((Direction)im.ReadByte(), im.ReadBoolean());
            }
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// Validate that an incoming message was sent from the user
        /// belonging to the current user player. If it wasnt, kick the
        /// sender
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        private Boolean ValidateSender(NetIncomingMessage im)
        {
            if (im.SenderConnection == this.driven.User.Connection)
                return true;

            im.SenderConnection.Disconnect("Invalid message. Goodbye.");

            return false;
        }
        #endregion
    }
}
