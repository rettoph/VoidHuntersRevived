using Guppy.DependencyInjection;
using Guppy.Extensions.System;
using Guppy.IO.Commands;
using Guppy.IO.Commands.Interfaces;
using Guppy.IO.Commands.Services;
using Guppy.Lists;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Utilities;
using Guppy.Network.Utilities.Messages;
using Guppy.Utilities;
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
using VoidHuntersRevived.Library.Extensions.Entities;
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
        private Synchronizer _synchronizer;
        #endregion

        #region Lifecycle Methods
        protected override void InitializeRemote(UserPlayer driven, ServiceProvider provider)
        {
            base.InitializeRemote(driven, provider);

            provider.Service(out _entities);
            provider.Service(out _world);
            provider.Service(out _synchronizer);

            _userConnection = provider.GetService<UserNetConnectionDictionary>().Connections[this.driven.User];

            this.driven.Ping.ValidateRead += this.ValidateReadAction;
            this.driven.Ping.Set(VHR.Network.Pings.Ship.UpdateTargetRequest, this.HandleUpdateShipTargetRequestMessage);
            this.driven.Ping.Set(VHR.Network.Pings.Ship.TractorBeam.ActionRequest, this.HandleShipTractorBeamActionRequestMessage);
            this.driven.Ping.Set(VHR.Network.Pings.Ship.UpdateDirectionRequest, this.HandleUpdateShipDirectionRequestMessage);
            this.driven.Ping.Set(VHR.Network.Pings.Ship.UpdateFiringRequest, this.HandleUpdateShipFiringRequestMessage);
            this.driven.Ping.Set(VHR.Network.Pings.Ship.SpawnRequest, this.HandleShipSpawnRequestMessage);
            this.driven.Ping.Set(VHR.Network.Pings.Ship.SpawnAiRequest, this.HandleSpawnAIRequestMessage);
            this.driven.Ping.Set(VHR.Network.Pings.Ship.SelfDestructRequest, this.HandleSelfDestructRequest);
            this.driven.Ping.Set(VHR.Network.Pings.Ship.LaunchDronesRequest, this.HandleLaunchDronesRequest);
            this.driven.Ping.Set(VHR.Network.Pings.Ship.ToggleEnergyShieldsRequest, this.HandleToggleEnergyShieldsRequest);
        }

        protected override void ReleaseRemote(UserPlayer driven)
        {
            base.ReleaseRemote(driven);

            _entities = null;
            _world = null;
            _userConnection = null;

            this.driven.Ping.ValidateRead -= this.ValidateReadAction;
            this.driven.Ping.Remove(VHR.Network.Pings.Ship.UpdateTargetRequest);
            this.driven.Ping.Remove(VHR.Network.Pings.Ship.TractorBeam.ActionRequest);
            this.driven.Ping.Remove(VHR.Network.Pings.Ship.UpdateDirectionRequest);
            this.driven.Ping.Remove(VHR.Network.Pings.Ship.UpdateFiringRequest);
            this.driven.Ping.Remove(VHR.Network.Pings.Ship.SpawnRequest);
            this.driven.Ping.Remove(VHR.Network.Pings.Ship.SpawnAiRequest);
            this.driven.Ping.Remove(VHR.Network.Pings.Ship.SelfDestructRequest);
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

        private void HandleSelfDestructRequest(NetIncomingMessage obj)
        {
            if (this.driven.Ship?.Bridge != default)
                this.driven.Ship.Bridge.Health = 0f;
        }

        private void HandleLaunchDronesRequest(NetIncomingMessage obj)
            => _synchronizer.Enqueue(gt => this.driven.Ship.TryLaunchFighters(gt));

        private void HandleToggleEnergyShieldsRequest(NetIncomingMessage obj)
            => _synchronizer.Enqueue(gt => this.driven.Ship.TryToggleEnergyShields(gt));
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
