﻿using Guppy;
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

            this.driven.Actions.TryAdd("direction:change:request", this.HandleDirectionChangeRequest);
            this.driven.Actions.TryAdd("tractor-beam:select:request", this.HandleTractorBeamSelectRequest);
            this.driven.Actions.TryAdd("tractor-beam:release:request", this.HandleTractorBeamReleaseRequest);
            this.driven.Actions.TryAdd("tractor-beam:attach:request", this.HandleTractorBeamAttachRequest);
        }
        #endregion

        #region Action Handlers
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
                this.driven.Ship.TractorBeam.TrySelect(im.ReadEntity<ShipPart>(_entities));
            }
        }

        private void HandleTractorBeamReleaseRequest(object sender, NetIncomingMessage im)
        {
            if (this.ValidateSender(im))
            { // If the message checks out...
                this.driven.Ship.SetTarget(im.ReadVector2());
                this.driven.Ship.TractorBeam.TryRelease();
            }
        }

        private void HandleTractorBeamAttachRequest(object sender, NetIncomingMessage im)
        {
            if (this.ValidateSender(im))
            { // If the message checks out...
                this.driven.Ship.SetTarget(im.ReadVector2());
                this.driven.Ship.TractorBeam.TryAttach(
                    node: im.ReadEntity<ShipPart>(_entities).FemaleConnectionNodes[im.ReadInt32()]);
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
