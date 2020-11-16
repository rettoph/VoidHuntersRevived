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

            if(provider.GetService<ClientPeer>().CurrentUser == driven.User)
            { // Setup the local user player driver now...
                provider.Service(out _commands);
                provider.Service(out _sensor);
                provider.Service(out _camera);
                provider.Service(out _synchronizer);

                _targetSender = new ActionTimer(50);

                this.driven.OnUpdate += this.Update;

                _commands["set"]["direction"].OnExcecute += this.HandleSetDirectionCommand;
                _commands["tractorbeam"].OnExcecute += this.HandleTractorBeamCommand;

                _configured = true;
            }
        }

        protected override void Release(UserPlayer driven)
        {
            base.Release(driven);

            if(_configured)
            {
                this.driven.OnUpdate -= this.Update;

                _commands["set"]["direction"].OnExcecute -= this.HandleSetDirectionCommand;
                _commands["tractorbeam"].OnExcecute -= this.HandleTractorBeamCommand;
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
                    this.WriteUpdateShipTargetRequest(this.driven.Actions.Create(NetDeliveryMethod.Unreliable, 8));
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

            _synchronizer.Enqueue(gt =>
            { // Broadcast a request message to the connected peer...
                if (response.Type != TractorBeam.ActionType.None)
                    this.WriteShipTractorBeamActionRequest(
                        om: this.driven.Actions.Create(NetDeliveryMethod.ReliableUnordered, 10),
                        action: response);
            });
        }
        #endregion

        #region Command Handlers
        private void HandleTractorBeamCommand(ICommand sender, CommandArguments args)
            => this.TryTractorBeamAction((TractorBeam.ActionType)args["action"]);

        private void HandleSetDirectionCommand(ICommand sender, CommandArguments args)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Network Methods
        private void WriteUpdateShipDirectionRequest(NetOutgoingMessage om, Ship.Direction direction, Boolean value)
            => om.Write("update:ship:direction:request", m =>
            {
                m.Write((Byte)direction);
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
                m.Write((Byte)action.Type);
                m.Write(action.Target);


                if (m.WriteExists(action.Target))
                {
                    m.Write(action.Target.Position);
                    m.Write(action.Target.Rotation);
                }
            });
        }
        #endregion
    }
}
