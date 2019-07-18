using Guppy.Implementations;
using Guppy.Network.Peers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Entities;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.CustomEventArgs;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Client.Library.Drivers
{
    /// <summary>
    /// The client user player driver is specifically for interfacing
    /// directly with external inputs and doing actions to the players
    /// ship.
    /// </summary>
    public class ClientUserPlayerDriver : Driver
    {
        #region Private Fields
        private FarseerCamera2D _camera;
        private Pointer _pointer;
        private ClientPeer _client;
        private UserPlayer _player;
        #endregion

        #region Constructors
        public ClientUserPlayerDriver(FarseerCamera2D camera, Pointer pointer, ClientPeer client, UserPlayer parent, IServiceProvider provider) : base(parent, provider)
        {
            _camera = camera;
            _pointer = pointer;
            _client = client;
            _player = parent;
        }
        #endregion

        #region Frame Methods
        protected override void update(GameTime gameTime)
        {
            base.update(gameTime);

            if (_client.CurrentUser == _player.User && _player.Ship != null)
            { // If the current player represents the locally signed in user...
                var kState = Keyboard.GetState();

                if(_player.Ship.Bridge != null)
                { // Bridge specific actions...

                    Boolean d;
                    if ((d = kState.IsKeyDown(Keys.W)) != _player.Ship.GetDirection(Direction.Forward))
                        this.SetLocalDirection(Direction.Forward, d);

                    // Update the camera position
                    _camera.MoveTo(_player.Ship.Bridge.Position);
                }

                // Update the camera zoom as needed...
                if(_pointer.ScrollDelta != 0)
                    _camera.ZoomBy(1 + 0.1f * ((Single)_pointer.ScrollDelta / 120));
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Update the local direction to a specified value,
        /// but also send a message to the server alerting it
        /// of this change.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="value"></param>
        private void SetLocalDirection(Direction direction, Boolean value)
        {
            _player.Ship.SetDirection(Direction.Forward, value);

            var action = _player.CreateActionMessage("set:direction");
            action.Write((Byte)direction);
            action.Write(value);
        }
        #endregion
    }
}
