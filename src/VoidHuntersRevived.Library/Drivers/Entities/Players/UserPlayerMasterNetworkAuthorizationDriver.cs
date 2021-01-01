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
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Extensions.Lidgren.Network;
using VoidHuntersRevived.Library.Extensions.System;

namespace VoidHuntersRevived.Library.Drivers.Entities.Players
{
    internal sealed class UserPlayerMasterNetworkAuthorizationDriver : MasterNetworkAuthorizationDriver<UserPlayer>
    {
        #region Private Fields
        private EntityList _entities;
        private NetConnection _userConnection;
        private WorldEntity _world;
        #endregion

        #region Lifecycle Methods
        protected override void InitializeRemote(UserPlayer driven, ServiceProvider provider)
        {
            base.InitializeRemote(driven, provider);

            provider.Service(out _entities);
            provider.Service(out _world);
            _userConnection = provider.GetService<UserNetConnectionDictionary>().Connections[this.driven.User];

            this.driven.Actions.ValidateRead += this.ValidateReadAction;
            this.driven.Actions.Set("update:ship:target:request", this.HandleUpdateShipTargetRequestMessage);
            this.driven.Actions.Set("ship:tractor-beam:action:request", this.HandleShipTractorBeamActionRequestMessage);
            this.driven.Actions.Set("update:ship:direction:request", this.HandleUpdateShipDirectionRequestMessage);
            this.driven.Actions.Set("update:ship:firing:request", this.HandleUpdateShipFiringRequestMessage);
            this.driven.Actions.Set("ship:spawn:request", this.HandleShipSpawnRequestMessage);
            this.driven.Actions.Set("spawn:ai:request", this.HandleSpawnAIRequestMessage);
        }

        protected override void ReleaseRemote(UserPlayer driven)
        {
            base.ReleaseRemote(driven);

            _entities = null;
            _world = null;
            _userConnection = null;

            this.driven.Actions.ValidateRead -= this.ValidateReadAction;
            this.driven.Actions.Remove("update:ship:target:request");
            this.driven.Actions.Remove("ship:tractor-beam:action:request");
            this.driven.Actions.Remove("update:ship:direction:request");
            this.driven.Actions.Remove("update:ship:firing:request");
            this.driven.Actions.Remove("ship:spawn:request");
        }
        #endregion

        #region Message Handlers
        private void HandleUpdateShipTargetRequestMessage(NetIncomingMessage im)
            => this.driven.Ship.ReadTarget(im);

        private void HandleShipTractorBeamActionRequestMessage(NetIncomingMessage im)
            => this.driven.Ship.TractorBeam.ReadAction(im);

        private void HandleUpdateShipDirectionRequestMessage(NetIncomingMessage im)
            => this.driven.Ship.ReadDirection(im);

        private void HandleUpdateShipFiringRequestMessage(NetIncomingMessage im)
            => this.driven.Ship.ReadFiring(im);

        private void HandleShipSpawnRequestMessage(NetIncomingMessage im)
        {
            var length = im.ReadInt32();
            var bytes = im.ReadBytes(length);
            var rand = new Random();
            this.driven.Ship.Import(
                data: bytes,
                position: rand.NextVector2(0, _world.Size.X, 0, _world.Size.Y),
                rotation: MathHelper.TwoPi * (Single)rand.Next());
        }

        private void HandleSpawnAIRequestMessage(NetIncomingMessage im)
        {
            var length = im.ReadInt32();
            var bytes = im.ReadBytes(length);
            var position = im.ReadVector2();
            var rotation = im.ReadSingle();

            _entities.Create<ComputerPlayer>((player, p, d) =>
            {
                player.Ship = _entities.Create<Ship>((ship, p2, c) =>
                {
                    ship.Import(
                        data: bytes,
                        position: position,
                        rotation: rotation);
                    });
            });
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
