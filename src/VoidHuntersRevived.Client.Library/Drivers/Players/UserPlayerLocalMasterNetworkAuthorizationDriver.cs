using Guppy.DependencyInjection;
using Guppy.IO.Commands;
using Guppy.IO.Commands.Interfaces;
using Guppy.IO.Commands.Services;
using Guppy.Lists;
using Guppy.Network.Peers;
using Guppy.Utilities;
using Guppy.Utilities.Cameras;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Client.Library.Entities;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Library.Drivers;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Extensions.Entities;
using VoidHuntersRevived.Library.Extensions.System;
using VoidHuntersRevived.Library.Lists;

namespace VoidHuntersRevived.Client.Library.Drivers.Players
{
    internal sealed class UserPlayerLocalMasterNetworkAuthorizationDriver : MasterNetworkAuthorizationDriver<UserPlayer>
    {
        #region Private Fields
        private Boolean _configured;
        private CommandService _commands;
        private Sensor _sensor;
        private Camera2D _camera;
        private Synchronizer _synchronizer;
        private EntityList _entities;
        private TeamList _teams;
        private WorldEntity _world;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(UserPlayer driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            if (provider.GetService<ClientPeer>().CurrentUser == driven.User)
            { // Setup the local user player driver now...
                provider.Service(out _commands);
                provider.Service(out _sensor);
                provider.Service(out  _camera);
                provider.Service(out _synchronizer);
                provider.Service(out _entities);
                provider.Service(out _teams);
                provider.Service(out _world);

                this.driven.OnUpdate += this.Update;
                this.driven.Ship.OnBridgeChanged += this.HandleShipBridgeChanged;

                _commands["ship"]["fire"].OnExcecute += this.TryHandleShipFireCommand;
                _commands["ship"]["direction"].OnExcecute += this.TryHandleShipDirectionCommand;
                _commands["ship"]["tractorbeam"].OnExcecute += this.TryHandleShipTractorBeamCommand;
                _commands["ship"]["save"].OnExcecute += this.TryHandleShipSaveCommand;
                _commands["ship"]["self-destruct"].OnExcecute += this.TryHandleShipSelfDestructCommand;
                _commands["ship"]["launch-fighters"].OnExcecute += this.TryHandleLaunchFightersCommand;
                _commands["ship"]["toggle-energy-shields"].OnExcecute += this.TryHandleToggleEnergyShieldsCommand;
                _commands["spawn"]["ai"].OnExcecute += this.TryHandleSpawnAICommand;

                _configured = true;
            }
        }

        protected override void Release(UserPlayer driven)
        {
            base.Release(driven);

            if (_configured)
            {
                this.driven.OnUpdate -= this.Update;
                this.driven.Ship.OnBridgeChanged -= this.HandleShipBridgeChanged;

                _commands["ship"]["fire"].OnExcecute -= this.TryHandleShipFireCommand;
                _commands["ship"]["direction"].OnExcecute -= this.TryHandleShipDirectionCommand;
                _commands["ship"]["tractorbeam"].OnExcecute -= this.TryHandleShipTractorBeamCommand;
                _commands["ship"]["save"].OnExcecute += this.TryHandleShipSaveCommand;
                _commands["ship"]["self-destruct"].OnExcecute -= this.TryHandleShipSelfDestructCommand;
                _commands["ship"]["launch-fighters"].OnExcecute -= this.TryHandleLaunchFightersCommand;
                _commands["ship"]["toggle-energy-shields"].OnExcecute -= this.TryHandleToggleEnergyShieldsCommand;
                _commands["spawn"]["ai"].OnExcecute -= this.TryHandleSpawnAICommand;
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

        }

        private void TrySetShipDirection(Ship.Direction direction, Boolean value)
        {
            this.driven.Ship.TrySetDirection(direction, value);
        }

        private void TrySetShipFiring(Boolean value)
        {
            this.driven.Ship.Firing = value;
        }
        #endregion

        #region Command Handlers
        private CommandResponse TryHandleShipFireCommand(ICommand sender, CommandInput input)
        {
            this.TrySetShipFiring((Boolean)input["value"]);


            return CommandResponse.Empty;
        }

        private CommandResponse TryHandleShipTractorBeamCommand(ICommand sender, CommandInput input)
        {
            this.TryTractorBeamAction((TractorBeam.ActionType)input["action"]);

            return CommandResponse.Empty;
        }

        private CommandResponse TryHandleShipDirectionCommand(ICommand sender, CommandInput input)
        {
            this.TrySetShipDirection((Ship.Direction)input["direction"], (Boolean)input["value"]);

            return CommandResponse.Empty;
        }

        private CommandResponse TryHandleShipSaveCommand(ICommand sender, CommandInput input)
        {
            Directory.CreateDirectory(VHR.Directories.Resources.Ships);

            using (FileStream file = File.Open($"Resources/Ships/{(input["name"] as String)}.vh", FileMode.OpenOrCreate))
                this.driven.Ship.Export().WriteTo(file);

            return CommandResponse.Success($"File saved at Resources/Ships/{(input["name"] as String)}.vh");
        }

        private CommandResponse TryHandleShipSelfDestructCommand(ICommand sender, CommandInput input)
        {
            if (this.driven.Ship?.Bridge != default)
                this.driven.Ship.Bridge.Health = 0f;

            return CommandResponse.Success("Requesting self destruct...");
        }

        private CommandResponse TryHandleLaunchFightersCommand(ICommand sender, CommandInput input)
        {
            _synchronizer.Enqueue(gt => this.driven.Ship.TryLaunchFighters(gt));

            return CommandResponse.Success("Requesting to launch fighters...");
        }

        private CommandResponse TryHandleToggleEnergyShieldsCommand(ICommand sender, CommandInput input)
        {
            _synchronizer.Enqueue(gt => this.driven.Ship.TryToggleEnergyShields(gt));

            return CommandResponse.Success("Requesting to toggle energy shields...");
        }
        #endregion

        #region Event Handlers
        private void HandleShipBridgeChanged(Ship sender, ShipPart old, ShipPart value)
        {
            if (value == default)
            {
                var ships = Directory.GetFiles(VHR.Directories.Resources.Ships, "*.vh");
                var rand = new Random();
                using (var fileStream = File.OpenRead(ships[rand.Next(ships.Length)]))
                {
                    this.driven.Ship.Import(fileStream, rand.NextVector2(0, _world.Size.X), MathHelper.TwoPi * (Single)rand.Next());
                }
            }
        }

        private CommandResponse TryHandleSpawnAICommand(ICommand sender, CommandInput input)
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

                    _entities.Create<ComputerPlayer>((player, p, d) =>
                    {
                        player.Team = _teams.GetNextTeam();
                        player.Ship = _entities.Create<Ship>((ship, p2, c) =>
                        {
                            ship.Import(
                                data: memoryStream.ToArray(),
                                position: position,
                                rotation: rotation);
                        });
                    });
                }

            }

            return CommandResponse.Success($"Attempting to spawn ai instance.");
        }
        #endregion
    }
}
