using Guppy.DependencyInjection;
using Guppy.Extensions.System;
using Guppy.IO.Commands;
using Guppy.IO.Commands.Interfaces;
using Guppy.IO.Commands.Services;
using Guppy.Lists;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Utilities;
using Guppy.Network.Utilities.Messages;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Drivers.Entities.Players
{
    internal sealed class UserPlayerMasterNetworkAuthorizationDriver : MasterNetworkAuthorizationDriver<UserPlayer>
    {
        #region Private Fields
        private EntityList _entities;
        private NetConnection _userConnection;
        private CommandService _commands;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(UserPlayer driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            provider.Service(out _entities);
            _userConnection = provider.GetService<UserNetConnectionDictionary>().Connections[this.driven.User];

            this.driven.Actions.ValidateRead += this.ValidateReadAction;
            this.driven.Actions.Set("update:ship:target:request", this.HandleUpdateShipTargetRequestMessage);
            this.driven.Actions.Set("ship:tractor-beam:action:request", this.HandleShipTractorBeamActionRequestMessage);
            this.driven.Actions.Set("update:ship:direction:request", this.HandleUpdateShipDirectionRequestMessage);
        }
        #endregion

        #region Message Handlers
        private void HandleUpdateShipTargetRequestMessage(NetIncomingMessage im)
            => this.driven.Ship.ReadTarget(im);

        private void HandleShipTractorBeamActionRequestMessage(NetIncomingMessage im)
        {
            this.driven.Ship.ReadTarget(im);

            var request = new TractorBeam.Action(
                type: (TractorBeam.ActionType)im.ReadByte(),
                target: im.ReadEntity<ShipPart>(_entities).Then(sp =>
                {
                    if (im.ReadBoolean())
                        sp.SetTransformIgnoreContacts(
                            position: im.ReadVector2(),
                            angle: im.ReadSingle());
                }));
            var response = this.driven.Ship.TractorBeam.TryAction(request);

            if(request.Type != response.Type)
            { // Something went wrong, so we need to alert the requesting client...

            }
        }

        private void HandleUpdateShipDirectionRequestMessage(NetIncomingMessage im)
        {
            this.driven.Ship.TrySetDirection(im.ReadEnum<Ship.Direction>(), im.ReadBoolean());
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// We must ensure that the requested UserPlayer action comes from 
        /// the connected Peer. Anything else shuld be rejected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private bool ValidateReadAction(MessageManager sender, NetIncomingMessage args)
        {
            if (args.SenderConnection == _userConnection)
                return true;

            // The sender is invalid...
            args.SenderConnection.Disconnect("Goodbye.");

            return false;
        }
        #endregion
    }
}
