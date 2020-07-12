using Guppy.DependencyInjection;
using Guppy.Network;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Peers;
using Guppy.UI.Entities;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using VoidHuntersRevived.Client.Library.Entities;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Drivers;
using VoidHuntersRevived.Library.Drivers.Entities;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities.Players
{
    /// <summary>
    /// Simple driver that will manage local controls for 
    /// players owned by the current client.
    /// </summary>
    internal sealed class UserPlayerLocalControllerDriver : NetworkEntityAuthorizationDriver<UserPlayer>
    {
        #region Private Fields
        private Peer _peer;
        private FarseerCamera2D _camera;
        private Sensor _sensor;
        private ActionTimer _targetSender;
        private Cursor _cursor;
        #endregion

        #region Lifecycle Methods
        protected override void Configure(Object driven, ServiceProvider provider)
        {
            base.Configure(driven, provider);

            _targetSender = new ActionTimer(50);

            provider.Service(out _peer);
            provider.Service(out _camera);
            provider.Service(out _sensor);
            provider.Service(out _cursor);

            this.driven.OnUserChanged += this.HandleUserChanged;
        }

        protected override void ConfigureLocal(ServiceProvider provider)
        {
            base.ConfigureLocal(provider);

            this.driven.OnUpdate += this.Update;
            _cursor.OnPressed += this.HandleCursorPressed;
            _cursor.OnReleased += this.HandleCursorReleased;
        }

        protected override void Dispose()
        {
            this.driven.OnUserChanged -= this.HandleUserChanged;
        }

        protected override void DisposeLocal()
        {
            base.DisposeLocal();

            this.driven.OnUpdate -= this.Update;
            _cursor.OnPressed -= this.HandleCursorPressed;
            _cursor.OnReleased -= this.HandleCursorReleased;
        }
        #endregion

        #region Frame Methods
        private void Update(GameTime gameTime)
        {
            if(this.driven.Ship != null)
            {
                // Update ship directions...
                var kState = Keyboard.GetState();
                this.TrySetShipDirection(Ship.Direction.Forward, kState.IsKeyDown(Keys.W));
                this.TrySetShipDirection(Ship.Direction.TurnRight, kState.IsKeyDown(Keys.D));
                this.TrySetShipDirection(Ship.Direction.Backward, kState.IsKeyDown(Keys.S));
                this.TrySetShipDirection(Ship.Direction.TurnLeft, kState.IsKeyDown(Keys.A));
                this.TrySetShipDirection(Ship.Direction.Left, kState.IsKeyDown(Keys.Q));
                this.TrySetShipDirection(Ship.Direction.Right, kState.IsKeyDown(Keys.E));

                this.driven.Ship.WorldTarget = _sensor.Position;
                _targetSender.Update(gameTime, () =>
                { // Attempt to send the newest target value...
                    this.WriteUpdateShipTargetRequest(this.driven.Actions.Create(NetDeliveryMethod.Unreliable, 8));
                });

                // Update camera position...
                _camera.MoveTo(this.driven.Ship.Bridge.WorldCenter);
            }
        }
        #endregion

        #region Helper Methods
        private void TrySetShipDirection(Ship.Direction direction, Boolean value)
        {
            if(this.driven.Ship.TrySetDirection(direction, value))
            { // If successful, broadcast a message request...
                this.WriteUpdateShipDirectionRequest(this.driven.Actions.Create(NetDeliveryMethod.ReliableUnordered, 7), direction, value);
            }
        }

        private void TrySelectShipTractorBeam()
        {
            var target = _sensor.Contacts
                .Where(c => c is ShipPart)
                .Select(c => (c as ShipPart).Controller is ChunkManager ? (c as ShipPart).Root : (c as ShipPart))
                .Where(s => this.driven.Ship.TractorBeam.CanSelect(s))
                .OrderBy(s => Vector2.Distance(this.driven.Ship.WorldTarget, s.WorldCenter))
                .FirstOrDefault();

            this.HandleTractorBeamAction(this.driven.Ship.TractorBeam.TrySelect(target));
        }

        private void TryDeselectShipTractorBeam()
            => this.HandleTractorBeamAction(this.driven.Ship.TractorBeam.TryDeselect(attach: true));

        /// <summary>
        /// Broadcast a tractorbeam action to the server in the form of an
        /// action request...
        /// </summary>
        /// <param name="action"></param>
        private void HandleTractorBeamAction(TractorBeam.Action action)
        {
            if(action.Type != TractorBeam.ActionType.None)
            { // Only send any request if its not none...
                this.WriteShipTractorBeamActionRequest(this.driven.Actions.Create(NetDeliveryMethod.ReliableUnordered, 10), action);
            }
        }
        #endregion

        #region Event Handlers
        private void HandleUserChanged(UserPlayer sender, User arg)
        {
            if(arg == _peer.CurrentUser)
            {
                // Give this specific user local authority...
                this.driven.Authorization = GameAuthorization.Local;
            }
        }

        private void HandleCursorPressed(Cursor sender, Cursor.Button arg)
            => this.TrySelectShipTractorBeam();

        private void HandleCursorReleased(Cursor sender, Cursor.Button arg)
            => this.TryDeselectShipTractorBeam();
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
                m.Write(this.driven.Ship.Target);
            });

        private void WriteShipTractorBeamActionRequest(NetOutgoingMessage om, TractorBeam.Action action)
        {
            om.Write("ship:tractor-beam:action:request", m =>
            {
                om.Write((Byte)action.Type);
                om.Write(action.Target);
            });
        }
        #endregion
    }
}
