using GalacticFighters.Library.Entities.Players;
using GalacticFighters.Library.Entities.ShipParts;
using Guppy;
using Guppy.Attributes;
using Guppy.Collections;
using Guppy.Network.Extensions.Lidgren;
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
        #region Private FIelds
        private EntityCollection _entities;
        #endregion

        #region Constructor
        public ServerUserPlayerDrivers(EntityCollection entities, UserPlayer driven) : base(driven)
        {
            _entities = entities;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.driven.Actions.TryAdd("direction:changed:request", this.HandleDirectionChangedRequest);
            this.driven.Actions.TryAdd("tractor-beam:selected:request", this.HandleTractorBeamSelectedRequest);
            this.driven.Actions.TryAdd("tractor-beam:released:request", this.HandleTractorBeamReleasedRequest);
        }
        #endregion

        #region Action Handlers
        private void HandleDirectionChangedRequest(object sender, NetIncomingMessage im)
        {
            if (this.ValidateSender(im))
            { // If the message checks out... update the ships direction.
                this.driven.Ship.SetDirection((Direction)im.ReadByte(), im.ReadBoolean());
            }
        }

        private void HandleTractorBeamSelectedRequest(object sender, NetIncomingMessage arg)
        {
            if(this.ValidateSender(arg))
                this.driven.Ship.TractorBeam.TrySelect(_entities.GetById<ShipPart>(arg.ReadGuid()));
        }

        private void HandleTractorBeamReleasedRequest(object sender, NetIncomingMessage arg)
        {
            if (this.ValidateSender(arg))
                this.driven.Ship.TractorBeam.TryRelease();
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