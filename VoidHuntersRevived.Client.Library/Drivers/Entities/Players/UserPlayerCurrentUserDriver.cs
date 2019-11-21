using Guppy;
using Guppy.Attributes;
using Guppy.Network.Peers;
using Guppy.UI.Entities;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Players;

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
        #endregion

        #region Constructor
        public UserPlayerCurrentUserDriver(Pointer pointer, FarseerCamera2D camera, ClientPeer client, UserPlayer driven) : base(driven)
        {
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

                this.UpdateDirection(Ship.Direction.Forward, kState.IsKeyDown(Keys.W));
                this.UpdateDirection(Ship.Direction.TurnRight, kState.IsKeyDown(Keys.D));
                this.UpdateDirection(Ship.Direction.Backward, kState.IsKeyDown(Keys.S));
                this.UpdateDirection(Ship.Direction.TurnLeft, kState.IsKeyDown(Keys.A));

                this.UpdateDirection(Ship.Direction.Left, kState.IsKeyDown(Keys.Q));
                this.UpdateDirection(Ship.Direction.Right, kState.IsKeyDown(Keys.E));

                // Update camera position
                _camera.MoveTo(this.driven.Ship.Bridge.Position);

            }
        }
        #endregion

        #region Input Handlers
        private void UpdateDirection(Ship.Direction direction, Boolean value)
        {
            if (this.driven.Ship.ActiveDirections.HasFlag(direction) != value)
            { // If the flag has not already been updated...
                // Update the local ship, so the local user feels immediate response...
                this.driven.Ship.SetDirection(direction, value);

                // Create an action to relay back to the server
                var action = this.driven.Actions.Create("direction:changed:request", NetDeliveryMethod.ReliableOrdered, 3);
                action.Write((Byte)direction);
                action.Write(value);
            }
        }
        #endregion

        #region Event Handlers
        private void HandlePointerScrolled(object sender, Int32 arg)
        { // Zoom in the camera
            _camera.ZoomTo((Single)Math.Pow(1.5, (Single)arg / 120));
        }
        #endregion
    }
}
