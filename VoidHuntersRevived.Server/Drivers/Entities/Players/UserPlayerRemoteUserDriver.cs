using Guppy;
using Guppy.Attributes;
using Guppy.Collections;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Server.Drivers.Entities.Players
{
    /// <summary>
    /// Manage and approve incoming request messages from remote
    /// UserPlayers.
    /// </summary>
    [IsDriver(typeof(UserPlayer))]
    internal sealed class UserPlayerRemoteUserDriver : Driver<UserPlayer>
    {
        #region Private Fields
        private EntityCollection _entities;
        #endregion

        #region Constructor
        public UserPlayerRemoteUserDriver(EntityCollection entities, UserPlayer driven) : base(driven)
        {
            _entities = entities;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.driven.Actions.TryAdd("target:change:request", this.HandleTargetChangeRequest);
            this.driven.Actions.TryAdd("firing:change:request", this.HandleFiringChangeRequest);
            this.driven.Actions.TryAdd("direction:change:request", this.HandleDirectionChangeRequest);
            this.driven.Actions.TryAdd("tractor-beam:select:request", this.HandleTractorBeamSelectRequest);
            this.driven.Actions.TryAdd("tractor-beam:release:request", this.HandleTractorBeamReleaseRequest);
            this.driven.Actions.TryAdd("tractor-beam:attach:request", this.HandleTractorBeamAttachRequest);
        }
        #endregion

        #region Action Handlers
        private void HandleTargetChangeRequest(object sender, NetIncomingMessage im)
        {
            if (this.ValidateSender(im))
            { // If the message checks out... update the ships target.
                this.driven.Ship.SetTarget(im.ReadVector2());
            }
        }

        private void HandleFiringChangeRequest(object sender, NetIncomingMessage im)
        {
            if (this.ValidateSender(im))
            { // If the message checks out... update the ships target.
                this.driven.Ship.SetTarget(im.ReadVector2());
                this.driven.Ship.SetFiring(im.ReadBoolean());
            }
        }

        private void HandleDirectionChangeRequest(object sender, NetIncomingMessage im)
        {
            if (this.ValidateSender(im))
            { // If the message checks out... update the ships direction.
                this.driven.Ship.SetDirection((Ship.Direction)im.ReadByte(), im.ReadBoolean());
            }
        }

        private void HandleTractorBeamSelectRequest(object sender, NetIncomingMessage im)
        {
            if (this.ValidateSender(im))
            { // If the message checks out...
                this.driven.Ship.SetTarget(im.ReadVector2());
                if(!this.driven.Ship.TractorBeam.TrySelect(im.ReadEntity<ShipPart>(_entities)))
                { // If the tractor beam is unable to select...
                    this.driven.Ship.Actions.Create("tractor-beam:select:request:denied", NetDeliveryMethod.ReliableOrdered, 4, im.SenderConnection); ;
                }
            }
        }

        private void HandleTractorBeamReleaseRequest(object sender, NetIncomingMessage im)
        {
            if (this.ValidateSender(im))
            { // If the message checks out...
                this.driven.Ship.SetTarget(im.ReadVector2());
                this.driven.Ship.TractorBeam.Rotation = im.ReadSingle();
                if(this.driven.Ship.TractorBeam.TryRelease() == default(ShipPart))
                { // If the tractor beam is unable to release...
                    this.driven.Ship.Actions.Create("tractor-beam:release:request:denied", NetDeliveryMethod.ReliableOrdered, 4, im.SenderConnection); ;
                }
            }
        }

        private void HandleTractorBeamAttachRequest(object sender, NetIncomingMessage im)
        {
            if (this.ValidateSender(im))
            { // If the message checks out...
                this.driven.Ship.SetTarget(im.ReadVector2());
                if(!this.driven.Ship.TractorBeam.TryAttach(im.ReadEntity<ShipPart>(_entities).FemaleConnectionNodes[im.ReadInt32()]))
                { // If the tractor beam is unable to attach
                    this.driven.Ship.Actions.Create("tractor-beam:select:attach:denied", NetDeliveryMethod.ReliableOrdered, 4, im.SenderConnection); ;
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
