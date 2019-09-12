using GalacticFighters.Client.Library.Scenes;
using GalacticFighters.Library.Entities;
using GalacticFighters.Library.Entities.Players;
using Guppy;
using Guppy.Attributes;
using Guppy.Network.Peers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Client.Library.Drivers.Entities.Players
{
    [IsDriver(typeof(UserPlayer))]
    public class ClientUserPlayerDriver : Driver<UserPlayer>
    {
        #region Private Fields
        private ClientPeer _client;
        private ClientGalacticFightersWorldScene _scene;
        #endregion

        #region Constructor
        public ClientUserPlayerDriver(ClientPeer client, ClientGalacticFightersWorldScene scene, UserPlayer driven) : base(driven)
        {
            _client = client;
            _scene = scene;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (this.driven.Ship.Bridge != null && this.driven.User == _client.User)
            { // If the current UserPlayer instance is owned by the local user...
                var kState = Keyboard.GetState();

                this.UpdateDirection(Ship.Direction.Forward, kState.IsKeyDown(Keys.W));
                this.UpdateDirection(Ship.Direction.Right, kState.IsKeyDown(Keys.D));
                this.UpdateDirection(Ship.Direction.Backward, kState.IsKeyDown(Keys.S));
                this.UpdateDirection(Ship.Direction.Left, kState.IsKeyDown(Keys.A));

                // Update the camera position
                _scene.Camera.MoveTo(this.driven.Ship.Bridge.WorldCenter);
            }
        }
        #endregion

        #region Input Handlers
        private void UpdateDirection(Ship.Direction direction, Boolean value)
        {
            if(this.driven.Ship.ActiveDirections.HasFlag(direction) != value)
            { // If the flag has not already been updated...
                // Update the local ship, so the local user feels immediate response...
                this.driven.Ship.SetDirection(direction, value);

                // Create an action to relay back to the server
                var action = this.driven.Actions.Create("direction:changed:request");
                action.Write((Byte)direction);
                action.Write(value);
            }
        }
        #endregion
    }
}
