using System;
using System.Collections.Generic;
using System.Text;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using VoidHuntersRevived.Client.Scenes;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.Drivers;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Client.Entities.Drivers
{
    class ClientUserPlayerDriver : Driver
    {
        #region Private Fields
        private Dictionary<MovementType, Boolean> _requestedMovement;
        private ClientMainGameScene _scene;
        #endregion

        #region Protected Fields
        protected UserPlayer UserPlayer;
        #endregion

        #region Constructors
        public ClientUserPlayerDriver(UserPlayer userPlayer, EntityInfo info, IGame game) : base(info, game)
        {
            this.UserPlayer = userPlayer;
        }
        #endregion

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            _scene = this.Scene as ClientMainGameScene;

            // Create a default movement array
            _requestedMovement = new Dictionary<MovementType, Boolean>(6);
            _requestedMovement.Add(MovementType.GoForward, false);
            _requestedMovement.Add(MovementType.TurnRight, false);
            _requestedMovement.Add(MovementType.GoBackward, false);
            _requestedMovement.Add(MovementType.TurnLeft, false);
            _requestedMovement.Add(MovementType.StrafeRight, false);
            _requestedMovement.Add(MovementType.StrafeLeft, false);

            // Ensure the player is always enabled
            this.SetEnabled(true);
        }
        #endregion

        #region Frame Methods
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(this.UserPlayer.User != null && this.UserPlayer.User.Id == _scene.Group.Peer.UniqueIdentifier)
            { // Only track the current user actions if they are the user in control of the current UserPlayer
                var keyboard = Keyboard.GetState();

                _requestedMovement[MovementType.GoForward]   = keyboard.IsKeyDown(Keys.W);
                _requestedMovement[MovementType.TurnRight]   = keyboard.IsKeyDown(Keys.D);
                _requestedMovement[MovementType.GoBackward]  = keyboard.IsKeyDown(Keys.S);
                _requestedMovement[MovementType.TurnLeft]    = keyboard.IsKeyDown(Keys.A);
                _requestedMovement[MovementType.StrafeRight] = keyboard.IsKeyDown(Keys.E);
                _requestedMovement[MovementType.StrafeLeft]  = keyboard.IsKeyDown(Keys.Q);

                // Update the camera position
                _scene.Camera.Position = this.UserPlayer.Bridge.Body.Position + Vector2.Transform(this.UserPlayer.Bridge.Body.LocalCenter, this.UserPlayer.Bridge.RotationMatrix);
                // The current UserPlayer should be synced every frame..
                if (!this.UserPlayer.Dirty)
                    this.UserPlayer.Dirty = true;
            }
        }
        #endregion



        #region Networking Methods (Driver Implementation)
        public override void Read(NetIncomingMessage im)
        {
            // Update the user Players user
            this.UserPlayer.User = _scene.Group.Users.GetById(im.ReadInt64());
        }

        public override void Write(NetOutgoingMessage om)
        {
            if (_requestedMovement == null) // No requested movement info to send, so no data to send
                om.Write(false);
            else
            { // Send the requested movement confirmation byte, then send the movement data
                om.Write(true);
                // Write the current requested movement settings
                foreach (KeyValuePair<MovementType, Boolean> kvp in _requestedMovement)
                {
                    om.Write((Byte)kvp.Key);
                    om.Write(kvp.Value);
                }
            }
        }
        #endregion
    }
}
