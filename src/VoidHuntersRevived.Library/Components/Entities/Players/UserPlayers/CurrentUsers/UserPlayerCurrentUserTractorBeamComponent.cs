﻿using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text;
using tainicom.Aether.Physics2D.Collision;
using VoidHuntersRevived.Library.Components.Entities.Ships;
using VoidHuntersRevived.Library.Entities.Aether;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Structs;
using VoidHuntersRevived.Library.Extensions.Lidgren;
using Guppy.DependencyInjection;
using Guppy.Network.Enums;
using Guppy.Network.Utilities;
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Library.Utilities;
using System.Linq;
using Guppy.Threading.Utilities;
using VoidHuntersRevived.Library.Globals.Constants;

namespace VoidHuntersRevived.Library.Components.Entities.Players
{
    internal sealed class UserPlayerCurrentUserTractorBeamComponent : UserPlayerCurrentUserCommandActionBaseComponent<TractorBeamAction>
    {
        #region Private Fields
        private AetherWorld _world;
        private ShipPartService _shipParts;
        #endregion

        #region Public Properties
        public override UInt32 ActionRequestMessageType => Messages.UserPlayer.RequestTractorBeamAction;

        public override String CurrentUserCommandInput => "ship tractorbeam";

        public override ICommandHandler CurrentUserCommandHandler => CommandHandler.Create<TractorBeamActionType, Single?, Single?, IConsole>(this.HandleShipTractorBeamCommand);
        #endregion

        #region Lifecycle Methods
        protected override void InitializeCurrentUser(GuppyServiceProvider provider)
        {
            base.InitializeCurrentUser(provider);

            provider.Service(out _world);
        }

        protected override void InitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            this.Entity.Messages[this.ActionRequestMessageType].OnRead += this.ReadRequestTractorBeamAction;
            this.Entity.Messages[this.ActionRequestMessageType].OnWrite += this.WriteRequestTractorBeamAction;

            base.InitializeRemote(provider, networkAuthorization);

            provider.Service(out _shipParts);
        }

        protected override void ReleaseRemote(NetworkAuthorization networkAuthorization)
        {
            base.ReleaseRemoteCurrentUser(networkAuthorization);

            _shipParts = default;

            this.Entity.Messages[this.ActionRequestMessageType].OnRead -= this.ReadRequestTractorBeamAction;
            this.Entity.Messages[this.ActionRequestMessageType].OnWrite -= this.WriteRequestTractorBeamAction;
        }

        protected override void ReleaseCurrentUser()
        {
            base.ReleaseCurrentUser();

            _world = default;
        }
        #endregion

        #region UserPlayerCurrentUserBaseComponent<TractorBeamAction> Implementation
        protected override Boolean TryDoActionRequest(TractorBeamAction request, out TractorBeamAction response)
        {
            response = this.Entity.Ship?.Components.Get<ShipTractorBeamComponent>().TryAction(request) ?? default;

            return response.Type != TractorBeamActionType.None;
        }

        protected override TractorBeamAction ReadCurrentUserActionRequestMessage(NetIncomingMessage im)
        {
            this.Entity.Ship.Components.Get<ShipTargetingComponent>().Target = im.ReadVector2();
            return im.ReadTractorBeamAction(_shipParts, ShipPartSerializationFlags.None);
        }

        protected override void WriteCurrentUserActionRequestMessage(NetOutgoingMessage om, TractorBeamAction request)
        {
            om.Write(this.Entity.Ship.Components.Get<ShipTargetingComponent>().Target);
            om.Write(request, _shipParts, ShipPartSerializationFlags.None);
        }
        #endregion

        #region Network Methods
        private void ReadRequestTractorBeamAction(MessageTypeManager sender, NetIncomingMessage im)
        {
            this.Entity.Ship.Components.Get<ShipTargetingComponent>().Target = im.ReadVector2();
        }

        private void WriteRequestTractorBeamAction(MessageTypeManager sender, NetOutgoingMessage om)
        {
            om.Write(this.Entity.Ship.Components.Get<ShipTargetingComponent>().Target);
        }
        #endregion

        #region Command Handlers
        private void HandleShipTractorBeamCommand(TractorBeamActionType action, Single? x, Single? y, IConsole arg3)
        {
            if (this.Entity.Ship == default)
                return;

            // Calculate the target position of the recieved input coords.
            Vector2 targetPosition = this.Entity.Ship.Components.Get<ShipTractorBeamComponent>().GetValidTractorbeamPosition(
                worldPosition: new Vector2(
                    x: x ?? this.Entity.Ship.Components.Get<ShipTargetingComponent>().Target.X,
                    y: y ?? this.Entity.Ship.Components.Get<ShipTargetingComponent>().Target.Y
                )
            );

            // Update the target position...
            this.Entity.Ship.Components.Get<ShipTargetingComponent>().Target = targetPosition;

            // Generate a request tractor beam action request based on recieved action type.
            TractorBeamAction request = action switch
            {
                TractorBeamActionType.Select => new TractorBeamAction(
                    type: TractorBeamActionType.Select,
                    targetShipPart: this.Entity.Ship.Components.Get<ShipTractorBeamComponent>().GetShipPartTarget(targetPosition)),
                TractorBeamActionType.Deselect => new TractorBeamAction(
                    type: TractorBeamActionType.Deselect),
                TractorBeamActionType.Attach => new TractorBeamAction(
                    type: TractorBeamActionType.Attach,
                    targetNode: this.Entity.Ship.Components.Get<ShipTractorBeamComponent>().GetConnectionNodeTarget(targetPosition)),
                _ => throw new ArgumentOutOfRangeException(nameof(action))
            };

            // Attempt the configured tractor beam action.
            this.TryDoActionRequest(request);
        }
        #endregion
    }
}
