using Guppy;
using Guppy.DependencyInjection;
using Guppy.IO.Commands;
using Guppy.IO.Commands.Interfaces;
using Guppy.IO.Commands.Services;
using Guppy.Network.Peers;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Client.Library.Entities;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Library.Utilities;
using Guppy.Utilities;
using System.IO;
using VoidHuntersRevived.Library.Extensions.Lidgren.Network;
using Guppy.Utilities.Cameras;
using VoidHuntersRevived.Library;

namespace VoidHuntersRevived.Client.Library.Drivers.Players
{
    internal sealed class UserPlayerLocalDriver : Driver<UserPlayer>
    {
        #region Private Fields
        private Boolean _configured;
        private CommandService _commands;
        private Sensor _sensor;
        private Camera2D _camera;
        private ActionTimer _targetSender;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(UserPlayer driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            if (provider.GetService<ClientPeer>().CurrentUser == driven.User)
            { // Setup the local user player driver now...
                provider.Service(out _commands);
                provider.Service(out _sensor);
                provider.Service(out _camera);

                _targetSender = new ActionTimer(50);

                this.driven.OnUpdate += this.Update;
                this.driven.Ship.OnBridgeChanged += this.HandleShipBridgeChanged;

                _commands["ship"]["fire"].OnExcecute += this.HandleShipFireCommand;
                _commands["ship"]["direction"].OnExcecute += this.HandleShipDirectionCommand;
                _commands["ship"]["tractorbeam"].OnExcecute += this.HandleShipTractorBeamCommand;
                _commands["ship"]["save"].OnExcecute += this.HandleShipSaveCommand;
                _commands["ship"]["self-destruct"].OnExcecute += this.HandleShipSelfDestructCommand;
                _commands["ship"]["launch-fighters"].OnExcecute += this.HandleLaunchFightersCommand;
                _commands["spawn"]["ai"].OnExcecute += this.HandleSpawnAICommand;

                _configured = true;
            }
        }

        protected override void Release(UserPlayer driven)
        {
            base.Release(driven);

            if(_configured)
            {
                this.driven.OnUpdate -= this.Update;
                this.driven.Ship.OnBridgeChanged -= this.HandleShipBridgeChanged;

                _commands["ship"]["fire"].OnExcecute -= this.HandleShipFireCommand;
                _commands["ship"]["direction"].OnExcecute -= this.HandleShipDirectionCommand;
                _commands["ship"]["tractorbeam"].OnExcecute -= this.HandleShipTractorBeamCommand;
                _commands["ship"]["save"].OnExcecute -= this.HandleShipSaveCommand;
                _commands["spawn"]["ai"].OnExcecute -= this.HandleSpawnAICommand;
            }

            _commands = null;
            _sensor = null;
            _camera = null;
        }
        #endregion

        #region Frame Methods
        private void Update(GameTime gameTime)
        {
            if (this.driven.Ship != null)
            {
                this.driven.Ship.Target = _sensor.Position;
                _targetSender.Update(gameTime, gt =>
                { // Attempt to send the newest target value...
                    this.WriteUpdateShipTargetRequest(this.driven.Ping.Create(NetDeliveryMethod.Unreliable, 0));
                });

                // Update camera position...
                _camera.MoveTo(this.driven.Ship.Bridge?.WorldCenter ?? _camera.PositionTarget);
            }
        }
        #endregion

        #region Helper Methods
        private void TryTractorBeamAction(TractorBeam.ActionType action)
        {
            var target = this.driven.Ship.TractorBeam.Selected?.Root ?? _sensor.Contacts
                .Where(c => c is ShipPart)
                .Select(c => (c as ShipPart).Chain.Controller is ChunkManager ? (c as ShipPart).Root : (c as ShipPart))
                .Where(s => this.driven.Ship.TractorBeam.CanSelect(s))
                .OrderBy(s => Vector2.Distance(this.driven.Ship.Target, s.WorldCenter))
                .FirstOrDefault();

            var response = this.driven.Ship.TractorBeam.TryAction(new TractorBeam.Action(action, target));
            if (response.Type != TractorBeam.ActionType.None)
                this.WriteShipTractorBeamActionRequest(
                    om: this.driven.Ping.Create(NetDeliveryMethod.ReliableUnordered, 0),
                    action: response);
        }

        private void TrySetShipDirection(Ship.Direction direction, Boolean value)
        {
            if (this.driven.Ship.TrySetDirection(direction, value))
            { // If successful, broadcast a message request...
                this.WriteUpdateShipDirectionRequest(this.driven.Ping.Create(NetDeliveryMethod.ReliableUnordered, 0), direction, value);
            }
        }

        private void TrySetShipFiring(Boolean value)
        {
            this.driven.Ship.Firing = value;

            this.WriteUpdateShipFiringRequest(this.driven.Ping.Create(NetDeliveryMethod.ReliableUnordered, 0), value);
        }
        #endregion

        #region Command Handlers
        private CommandResponse HandleShipFireCommand(ICommand sender, CommandInput input)
        {
            this.TrySetShipFiring((Boolean)input["value"]);
            

            return CommandResponse.Empty;
        }

        private CommandResponse HandleShipTractorBeamCommand(ICommand sender, CommandInput input)
        {
            this.TryTractorBeamAction((TractorBeam.ActionType)input["action"]);

            return CommandResponse.Empty;
        }

        private CommandResponse HandleShipDirectionCommand(ICommand sender, CommandInput input)
        {
            this.TrySetShipDirection((Ship.Direction)input["direction"], (Boolean)input["value"]);

            return CommandResponse.Empty;
        }

        private CommandResponse HandleShipSaveCommand(ICommand sender, CommandInput input)
        {
            Directory.CreateDirectory(VHR.Directories.Resources.Ships);

            using (FileStream file = File.Open($"Resources/Ships/{(input["name"] as String)}.vh", FileMode.OpenOrCreate))
                this.driven.Ship.Export().WriteTo(file);

            return CommandResponse.Success($"File saved at Resources/Ships/{(input["name"] as String)}.vh");
        }

        private CommandResponse HandleShipSelfDestructCommand(ICommand sender, CommandInput input)
        {
            this.driven.Ping.Create(NetDeliveryMethod.ReliableUnordered, 0).Write(VHR.Network.Pings.Ship.SelfDestructRequest, m => { });

            return CommandResponse.Success("Requesting self destruct...");
        }

        private CommandResponse HandleLaunchFightersCommand(ICommand sender, CommandInput input)
        {
            this.driven.Ping.Create(NetDeliveryMethod.ReliableUnordered, 0).Write(VHR.Network.Pings.Ship.LaunchFightersRequest, m => { });

            return CommandResponse.Success("Requesting to launch fighters...");
        }
        #endregion

        #region Network Methods
        private void WriteUpdateShipFiringRequest(NetOutgoingMessage om, Boolean value)
            => om.Write(VHR.Network.Pings.Ship.UpdateFiringRequest, m =>
            {
                this.driven.Ship.WriteFiring(m);
            });

        private void WriteUpdateShipDirectionRequest(NetOutgoingMessage om, Ship.Direction direction, Boolean value)
            => om.Write(VHR.Network.Pings.Ship.UpdateDirectionRequest, m =>
            {
                m.Write(direction);
                m.Write(value);
            });

        private void WriteUpdateShipTargetRequest(NetOutgoingMessage om)
            => om.Write(VHR.Network.Pings.Ship.UpdateTargetRequest, m =>
            {
                this.driven.Ship.WriteTarget(m);
            });

        private void WriteShipTractorBeamActionRequest(NetOutgoingMessage om, TractorBeam.Action action)
        {
            om.Write(VHR.Network.Pings.Ship.TractorBeam.ActionRequest, m =>
            {
                this.driven.Ship.TractorBeam.WriteAction(m, action);
            });
        }
        #endregion

        #region Event Handlers
        private void HandleShipBridgeChanged(Ship sender, ShipPart old, ShipPart value)
        {
            if(value == default)
            {
                this.driven.Ping.Create(NetDeliveryMethod.ReliableOrdered, 10).Write(VHR.Network.Pings.Ship.SpawnRequest, m =>
                {
                    var ships = Directory.GetFiles(VHR.Directories.Resources.Ships, "*.vh");
                    var rand = new Random();
                    using (var fileStream = File.OpenRead(ships[rand.Next(ships.Length)]))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            fileStream.CopyTo(memoryStream);
                            var bytes = memoryStream.ToArray();
                            m.Write(bytes.Length);
                            m.Write(bytes);
                        }
                    }
                });
            }
        }

        private CommandResponse HandleSpawnAICommand(ICommand sender, CommandInput input)
        {
            this.driven.Ping.Create(NetDeliveryMethod.ReliableOrdered, 10).Write(VHR.Network.Pings.Ship.SpawnAiRequest, m =>
            {
                var ships = Directory.GetFiles(VHR.Directories.Resources.Ships, "*.vh");
                var rand = new Random();
                var position = new Vector2(
                    input.GetIfContains<Single>("positionX", _sensor.Position.X), 
                    input.GetIfContains<Single>("positionY", _sensor.Position.Y));
                var rotation = input.GetIfContains<Single>("rotation", MathHelper.TwoPi * (Single)rand.Next());

                using (var fileStream = input.GetIfContains<FileStream>("name", File.OpenRead(ships[rand.Next(ships.Length)])))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        fileStream.CopyTo(memoryStream);
                        var bytes = memoryStream.ToArray();
                        m.Write(bytes.Length);
                        m.Write(bytes);
                        m.Write(position);
                        m.Write(rotation);
                    }
                }
            });

            return CommandResponse.Success($"Attempting to spawn ai instance.");
        }
        #endregion
    }
}
