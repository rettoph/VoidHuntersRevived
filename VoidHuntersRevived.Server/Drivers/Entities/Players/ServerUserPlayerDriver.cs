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
            this.driven.Actions.TryAdd("tractor-beam:attached:request", this.HandleTractorBeamAttachedRequest);
            this.driven.Actions.TryAdd("target:changed:request", this.HandleTargetChangedRequest);
        }
        #endregion

        #region Action Handlers
        private void HandleTargetChangedRequest(object sender, NetIncomingMessage im)
        {
            if (this.ValidateSender(im))
            { // If the message checks out... update the ships direction.
                this.driven.Ship.ReadTargetOffset(im);
            }
        }

        private void HandleDirectionChangedRequest(object sender, NetIncomingMessage im)
        {
            if (this.ValidateSender(im))
            { // If the message checks out... update the ships direction.
                this.driven.Ship.SetDirection((Direction)im.ReadByte(), im.ReadBoolean());
            }
        }

        private void HandleTractorBeamSelectedRequest(object sender, NetIncomingMessage arg)
        {
            if (this.ValidateSender(arg))
            {
                this.driven.Ship.ReadTargetOffset(arg);
                if(!this.driven.Ship.TractorBeam.TrySelect(_entities.GetById<ShipPart>(arg.ReadGuid())))
                { // Create a denied message & send to the client
                    this.driven.Actions.Create("tractor-beam:selected:denied", arg.SenderConnection);
                }
            }
        }

        private void HandleTractorBeamReleasedRequest(object sender, NetIncomingMessage arg)
        {
            if (this.ValidateSender(arg))
            {
                this.driven.Ship.ReadTargetOffset(arg);
                if (!this.driven.Ship.TractorBeam.TryRelease())
                { // Create a denied message & send to the client
                    this.driven.Actions.Create("tractor-beam:released:denied", arg.SenderConnection);
                }
            }
        }

        private void HandleTractorBeamAttachedRequest(object sender, NetIncomingMessage arg)
        {
            if (this.ValidateSender(arg))
            {
                // Attempt to attach the the object recieved by the client...
                if (!this.driven.Ship.TractorBeam.TryAttach(arg.ReadEntity<ShipPart>(_entities).FemaleConnectionNodes[arg.ReadInt32()]))
                { // Create a denied message & send to the client
                    this.driven.Actions.Create("tractor-beam:attached:denied", arg.SenderConnection);
                }
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