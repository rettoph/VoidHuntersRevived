using Guppy;
using Guppy.Attributes;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Players;

namespace VoidHuntersRevived.Server.Drivers.Entities.Players
{
    /// <summary>
    /// Manage and approve incoming request messages from remote
    /// UserPlayers.
    /// </summary>
    [IsDriver(typeof(UserPlayer))]
    internal sealed class UserPlayerRemoteUserDriver : Driver<UserPlayer>
    {
        #region Constructor
        public UserPlayerRemoteUserDriver(UserPlayer driven) : base(driven)
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
            if (this.ValidateSender(im))
            { // If the message checks out... update the ships direction.
                this.driven.Ship.SetDirection((Ship.Direction)im.ReadByte(), im.ReadBoolean());
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
