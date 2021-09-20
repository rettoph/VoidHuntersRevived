using Guppy.CommandLine.Services;
using Guppy.DependencyInjection;
using Guppy.Enums;
using Guppy.Interfaces;
using Guppy.IO.Services;
using Guppy.Network.Components;
using Guppy.Network.Interfaces;
using Guppy.Network.Peers;
using Guppy.Utilities.Cameras;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.Text;
using VoidHuntersRevived.Library.Components.Entities.Ships;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Interfaces;
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Library.Entities.Ships;

namespace VoidHuntersRevived.Client.Library.Components.Entities.Players
{
    internal sealed class UserPlayerLocalComponent : RemoteHostComponent<UserPlayer>
    {
        #region Private Fields
        private Camera2D _camera;
        private ClientPeer _client;
        private CommandService _commands;
        private MouseService _mouse;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);

            provider.Service(out _camera);
            provider.Service(out _client);
            provider.Service(out _commands);
            provider.Service(out _mouse);

            if (_client.CurrentUser == this.Entity.User)
            {
                _commands.Get<Command>("ship set direction").Handler = CommandHandler.Create<Direction, Boolean, IConsole>(this.ShipSetDirectionHandler);

                this.Entity.OnUpdate += this.Update;
            }
            else
            {
                this.Entity.ChunkLoader = false;
            }
        }

        protected override void Release()
        {
            base.Release();

            this.Entity.OnUpdate -= this.Update;

            _client = default;
            _camera = default;
        }
        #endregion

        #region Frame Methods
        private void Update(GameTime gameTime)
        {
            if (this.Entity.Ship == default)
                return;

            _camera.MoveTo(this.Entity.Ship.Chain.Position);

            this.Entity.Ship.Components.Get<ShipTargetingComponent>().Target = _camera.Unproject(_mouse.Position);
        }
        #endregion

        #region Command Handlers
        private void ShipSetDirectionHandler(Direction direction, Boolean value, IConsole arg3)
        {
            // TODO: Only broadcast when connected to remote peer.
            if(this.Entity.Ship?.Components.Get<ShipDirectionComponent>().TrySetDirection(direction, value) ?? false)
            { // Broadcast a message to the server alerting it of the directional update...
                this.Entity.Messages[VoidHuntersRevived.Library.Constants.Messages.UserPlayer.RequestDirectionChanged].Create(
                    writer: om =>
                    {
                        om.Write<Direction>(direction);
                        om.Write(value);
                    },
                    recipients: this.Entity.Pipe);
            }
        }
        #endregion
    }
}
