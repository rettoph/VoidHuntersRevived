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
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Utilities;
using Guppy.Utilities;
using System.IO;
using VoidHuntersRevived.Library.Extensions.Lidgren.Network;

namespace VoidHuntersRevived.Client.Library.Drivers.Players
{
    internal sealed class UserPlayerLocalDriver : Driver<UserPlayer>
    {
        #region Private Fields
        private Boolean _configured;
        private CommandService _commands;
        private Sensor _sensor;
        private FarseerCamera2D _camera;
        private ActionTimer _targetSender;
        private Synchronizer _synchronizer;
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
                provider.Service(out _synchronizer);

                _targetSender = new ActionTimer(50);

                this.driven.OnUpdate += this.Update;

                _commands["ship"]["direction"].OnExcecute += this.HandleShipDirectionCommand;
                _commands["ship"]["tractorbeam"].OnExcecute += this.HandleShipTractorBeamCommand;
                _commands["ship"]["save"].OnExcecute += this.HandleShipSaveCommand;

                _configured = true;
            }
        }

        protected override void Release(UserPlayer driven)
        {
            base.Release(driven);

            if(_configured)
            {
                this.driven.OnUpdate -= this.Update;

                _commands["ship"]["direction"].OnExcecute -= this.HandleShipDirectionCommand;
                _commands["ship"]["tractorbeam"].OnExcecute -= this.HandleShipTractorBeamCommand;
            }
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
                    this.WriteUpdateShipTargetRequest(this.driven.Actions.Create(NetDeliveryMethod.Unreliable, 0));
                });

                // Update camera position...
                _camera.MoveTo(this.driven.Ship.Bridge.WorldCenter);
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
                    om: this.driven.Actions.Create(NetDeliveryMethod.ReliableUnordered, 0),
                    action: response);
        }

        private void TrySetShipDirection(Ship.Direction direction, Boolean value)
        {
            if (this.driven.Ship.TrySetDirection(direction, value))
            { // If successful, broadcast a message request...
                this.WriteUpdateShipDirectionRequest(this.driven.Actions.Create(NetDeliveryMethod.ReliableUnordered, 0), direction, value);
            }
        }
        #endregion

        #region Command Handlers
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
            Directory.CreateDirectory("Ships");

            using (FileStream file = File.Open($"Ships/{(input["name"] as String)}.vh", FileMode.OpenOrCreate))
                this.driven.Ship.Export().WriteTo(file);

            return CommandResponse.Success();
        }
        #endregion

        #region Network Methods
        private void WriteUpdateShipDirectionRequest(NetOutgoingMessage om, Ship.Direction direction, Boolean value)
            => om.Write("update:ship:direction:request", m =>
            {
                m.Write(direction);
                m.Write(value);
            });

        private void WriteUpdateShipTargetRequest(NetOutgoingMessage om)
            => om.Write("update:ship:target:request", m =>
            {
                this.driven.Ship.WriteTarget(m);
            });

        private void WriteShipTractorBeamActionRequest(NetOutgoingMessage om, TractorBeam.Action action)
        {
            om.Write("ship:tractor-beam:action:request", m =>
            {
                this.driven.Ship.WriteTarget(m);
                m.Write(action.Type);
                m.Write(action.TargetPart, (m, e) =>
                {
                    m.Write(action.TargetPart.Position);
                    m.Write(action.TargetPart.Rotation);
                });
                m.Write(action.TargetNode);
            });
        }
        #endregion
    }
}
