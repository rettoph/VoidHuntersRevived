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

namespace VoidHuntersRevived.Client.Library.Drivers.Entities.Players
{
    /// <summary>
    /// Driver primarily in charge of default player controls
    /// for the local user.
    /// </summary>
    [IsDriver(typeof(UserPlayer))]
    internal sealed class UserPlayerCurrentUserDriver : Driver<UserPlayer>
    {
        #region Private Fields
        private ClientPeer _client;
        private Action<GameTime> _update;
        private FarseerCamera2D _camera;
        private Pointer _pointer;
        private Sensor _sensor;
        #endregion

        #region Constructor
        public UserPlayerCurrentUserDriver(Sensor sensor, Pointer pointer, FarseerCamera2D camera, ClientPeer client, UserPlayer driven) : base(driven)
        {
            _sensor = sensor;
            _camera = camera;
            _client = client;
            _pointer = pointer;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            if(_client.User == this.driven.User)
            {
                _update = this.LocalUpdate;
                // _camera.ZoomLerp = 0.005f;

                // Setup local user events
                _pointer.Events.TryAdd<Int32>("scrolled", this.HandlePointerScrolled);
                _pointer.Events.TryAdd<Pointer.Button>("pressed", this.HandlePointerButtonPressed);
                _pointer.Events.TryAdd<Pointer.Button>("released", this.HandlePointerButtonReleased);
            }
            else
            {
                _update = gt =>
                {
                    // Empty method...
                };
            }
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
                _camera.MoveTo(this.driven.Ship.Bridge.Position);
                // Update the ship's target position
                this.driven.Ship.SetTarget(_sensor.WorldCenter - this.driven.Ship.Bridge.WorldCenter);
            }
        }
        #endregion

        #region Input Handlers
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
                .Where(sp => sp is ShipPart && this.driven.Ship.TractorBeam.ValidateTarget(sp as ShipPart))
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
            if (this.driven.Ship.TractorBeam.TryRelease())
            { // Write a release action to the server
                var action = this.driven.Actions.Create("tractor-beam:release:request", NetDeliveryMethod.ReliableOrdered, 3);
                action.Write(this.driven.Ship.Target);
            }
        }
        #endregion

        #region Event Handlers
        private void HandlePointerScrolled(object sender, Int32 arg)
        { // Zoom in the camera
            _camera.ZoomTo((Single)Math.Pow(1.5, (Single)arg / 120));
        }

        private void HandlePointerButtonPressed(object sender, Pointer.Button button)
        {
            switch (button)
            {
                case Pointer.Button.Left:
                    // this.TrySetFiring(true);
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
                    // this.TrySetFiring(false);
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
