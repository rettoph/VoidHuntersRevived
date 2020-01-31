using Guppy;
using Guppy.Attributes;
using Guppy.Network.Peers;
using Guppy.UI.Entities;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Client.Library.Entities;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Library.Utilities;
using Microsoft.Extensions.Logging;
using System.IO;
using VoidHuntersRevived.Client.Library.Utilities;
using Guppy.UI.Utilities;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities.Players
{
    /// <summary>
    /// Driver primarily in charge of default player controls
    /// for the local user.
    /// </summary>
    [IsDriver(typeof(UserPlayer))]
    internal sealed class UserPlayerCurrentUserDriver : Driver<UserPlayer>
    {
        #region Static Attributes
        private static Double TargetPingRate { get; set; } = 75;
        #endregion

        #region Private Fields
        private ClientPeer _client;
        private Action<GameTime> _update;
        private FarseerCamera2D _camera;
        private Pointer _pointer;
        private Sensor _sensor;
        private ActionTimer _targetPingTimer;
        private ShipBuilder _shipBuilder;
        private DebugOverlay _debug;
        private Boolean _wasDown;
        private PopupManager _popupManager;
        private Single _oldScroll;
        #endregion

        #region Constructor
        public UserPlayerCurrentUserDriver(PopupManager popupManager, DebugOverlay debug, ShipBuilder shipBuilder, Sensor sensor, Pointer pointer, FarseerCamera2D camera, ClientPeer client, UserPlayer driven) : base(driven)
        {
            _sensor = sensor;
            _camera = camera;
            _client = client;
            _pointer = pointer;
            _shipBuilder = shipBuilder;
            _debug = debug;
            _popupManager = popupManager;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            if(_client.User == this.driven.User)
            {
                _targetPingTimer = new ActionTimer(UserPlayerCurrentUserDriver.TargetPingRate);
                _update = this.LocalUpdate;
                // _camera.ZoomLerp = 0.005f;

                // Setup local user events
                _pointer.OnScrolled += this.HandlePointerScrolled;
                _pointer.OnPressed += this.HandlePointerButtonPressed;
                _pointer.OnReleased += this.HandlePointerButtonReleased;


                _debug.AddLine(gt => "\nCurrent Player");
                _debug.AddLine(gt => $"  Position => X: {this.driven?.Ship?.Bridge?.Position.X.ToString("#,##0.000")} Y: {this.driven?.Ship?.Bridge?.Position.Y.ToString("#,##00.000")}, R: {this.driven?.Ship?.Bridge?.Rotation.ToString("#,##00.000")}");
                _debug.AddLine(gt => $"  Target => X: {this.driven?.Ship?.WorldTarget.X.ToString("#,##00.000")} Y: {this.driven?.Ship?.WorldTarget.X.ToString("#,##00.000")}");
                _debug.AddLine(gt => $"  Energy => {this.driven?.Ship?.Energy.ToString("#,##00.000")}");
            }
            else
            {
                _update = gt =>
                {
                    // Empty method...
                };
            }
        }

        protected override void Dispose()
        {
            base.Dispose();

            _pointer.OnScrolled -= this.HandlePointerScrolled;
            _pointer.OnPressed -= this.HandlePointerButtonPressed;
            _pointer.OnReleased -= this.HandlePointerButtonReleased;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _update.Invoke(gameTime);
        }

        private void LocalUpdate(GameTime gameTime)
        {
            if (this.driven.Ship != null && this.driven.Ship.Bridge != null)
            {
                var kState = Keyboard.GetState();

                this.TryUpdateDirection(Ship.Direction.Forward, kState.IsKeyDown(Keys.W));
                this.TryUpdateDirection(Ship.Direction.TurnRight, kState.IsKeyDown(Keys.D));
                this.TryUpdateDirection(Ship.Direction.Backward, kState.IsKeyDown(Keys.S));
                this.TryUpdateDirection(Ship.Direction.TurnLeft, kState.IsKeyDown(Keys.A));

                this.TryUpdateDirection(Ship.Direction.Left, kState.IsKeyDown(Keys.Q));
                this.TryUpdateDirection(Ship.Direction.Right, kState.IsKeyDown(Keys.E));

                // Update camera position
                _camera.MoveTo(this.driven.Ship.Bridge.WorldCenter);
                // Update the ship's target position
                this.TrySetTarget(_sensor.WorldCenter - this.driven.Ship.Bridge.WorldCenter, gameTime);

                if(!_wasDown)
                {
                    if(kState.IsKeyDown(Keys.F))
                    {
                        var action = this.driven.Actions.Create("spawn:request", NetDeliveryMethod.ReliableOrdered, 0);
                        action.Write(false);
                        action.Write(this.driven.Ship.WorldTarget);
                    }
                    else if(kState.IsKeyDown(Keys.G))
                    {
                        var action = this.driven.Actions.Create("spawn:request", NetDeliveryMethod.ReliableOrdered, 0);
                        action.Write(true);
                        var output = _shipBuilder.Export(this.driven.Ship.Bridge).ToArray();
                        action.Write(output.Length);
                        action.Write(output);
                        action.Write(this.driven.Ship.WorldTarget);
                    }
                }

                _wasDown = kState.IsKeyDown(Keys.F) || kState.IsKeyDown(Keys.G);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // Detect the current hovered ShipPart if any
            

            if (!this.driven.Ship.Firing && this.driven.Ship.TractorBeam.Selected == default(ShipPart))
            {
                var target = _sensor.Contacts
                    .Where(sp => sp is ShipPart && (sp as ShipPart).Root.Ship != this.driven.Ship && this.driven.Ship.TractorBeam.FindTarget(sp as ShipPart) != default(ShipPart))
                    .OrderBy(sp => Vector2.Distance(sp.WorldCenter, _sensor.WorldCenter))
                    .FirstOrDefault() as ShipPart;

                _popupManager.SetHovered(target);
            }
            else
                _popupManager.SetHovered(default(ShipPart));
        }
        #endregion

        #region Input Handlers
        private void TrySetTarget(Vector2 target, GameTime gameTime)
        {
            this.driven.Ship.SetTarget(target);

            _targetPingTimer.Update(gameTime, () =>
            { // On the interval...
                // Create an action to relay back to the server with the clients most up to date target
                var action = this.driven.Actions.Create("target:change:request", NetDeliveryMethod.ReliableOrdered, 3);
                action.Write(this.driven.Ship.Target);
            });
        }
        private void TrySetFiring(Boolean value)
        {
            if (this.driven.Ship.Firing != value)
            { // If the flag has not already been updated...
                // Update the local ship, so the local user feels immediate response...
                this.driven.Ship.SetFiring(value);

                // Create an action to relay back to the server
                var action = this.driven.Actions.Create("firing:change:request", NetDeliveryMethod.ReliableOrdered, 3);
                action.Write(this.driven.Ship.Target);
                action.Write(value);
            }
        }

        private void TryUpdateDirection(Ship.Direction direction, Boolean value)
        {
            if (this.driven.Ship.ActiveDirections.HasFlag(direction) != value)
            { // If the flag has not already been updated...
                // Update the local ship, so the local user feels immediate response...
                this.driven.Ship.SetDirection(direction, value);

                // Create an action to relay back to the server
                var action = this.driven.Actions.Create("direction:change:request", NetDeliveryMethod.ReliableOrdered, 3);
                action.Write((Byte)direction);
                action.Write(value);
            }
        }

        private void TrySelectTractorBeam()
        {
            // Immediately attempt to select the local tractorbeam
            var target = _sensor.Contacts
                .Where(sp => sp is ShipPart && this.driven.Ship.TractorBeam.FindTarget(sp as ShipPart) != default(ShipPart))
                .OrderBy(sp => Vector2.Distance(sp.WorldCenter, _sensor.WorldCenter))
                .FirstOrDefault() as ShipPart;

            if (this.driven.Ship.TractorBeam.TrySelect(target))
            { // Write an action to the server...
                var action = this.driven.Actions.Create("tractor-beam:select:request", NetDeliveryMethod.ReliableOrdered, 3);
                action.Write(this.driven.Ship.Target);
                action.Write(this.driven.Ship.TractorBeam.Selected);
            }
        }

        private void TryReleaseTractorBeam()
        {
            var node = this.driven.Ship.GetClosestOpenFemaleNode(this.driven.Ship.TractorBeam.Position);

            if(node == default(ConnectionNode))
            { // If there is no valid connection node in range...
                if (this.driven.Ship.TractorBeam.TryRelease() != default(ShipPart))
                { // Write a release action to the server
                    var action = this.driven.Actions.Create("tractor-beam:release:request", NetDeliveryMethod.ReliableOrdered, 3);
                    action.Write(this.driven.Ship.Target);
                    action.Write(this.driven.Ship.TractorBeam.Rotation);
                }
            }
            else
            { // If there is a valid connection node in range...
                if (this.driven.Ship.TractorBeam.TryAttach(node))
                { // Write a release action to the server
                    var action = this.driven.Actions.Create("tractor-beam:attach:request", NetDeliveryMethod.ReliableOrdered, 3);
                    action.Write(this.driven.Ship.Target);
                    action.Write(node.Parent);
                    action.Write(node.Id);
                }
            }

        }
        #endregion

        #region Event Handlers
        private void HandlePointerScrolled(object sender, Single arg)
        { // Zoom in the camera
            _camera.ZoomTo(MathHelper.Clamp(_camera.Zoom * (Single)Math.Pow(1.5, (arg - _oldScroll) / 120), 0.0225f, 0.5f));
            _oldScroll = arg;
        }

        private void HandlePointerButtonPressed(object sender, Pointer.Button button)
        {
            // Export & save the ship in its current state...
            using (FileStream output = File.OpenWrite("ship.vh"))
                _shipBuilder.Export(this.driven.Ship.Bridge).WriteTo(output);

            switch (button)
            {
                case Pointer.Button.Left:
                    this.TrySetFiring(true);
                    break;
                case Pointer.Button.Middle:
                    break;
                case Pointer.Button.Right:
                    this.TrySelectTractorBeam();
                    break;
            }
        }

        private void HandlePointerButtonReleased(object sender, Pointer.Button button)
        {
            switch (button)
            {
                case Pointer.Button.Left:
                    this.TrySetFiring(false);
                    break;
                case Pointer.Button.Middle:
                    break;
                case Pointer.Button.Right:
                    this.TryReleaseTractorBeam();
                    break;
            }
        }
        #endregion
    }
}
