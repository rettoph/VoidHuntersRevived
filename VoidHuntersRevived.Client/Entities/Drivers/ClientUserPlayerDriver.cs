using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics;
using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using VoidHuntersRevived.Client.Scenes;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.Drivers;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;
using System.Linq;
using VoidHuntersRevived.Library.Entities;
using Microsoft.Extensions.Logging;

namespace VoidHuntersRevived.Client.Entities.Drivers
{
    public class ClientUserPlayerDriver : Driver
    {
        #region Private Fields
        private Dictionary<MovementType, Boolean> _requestedMovement;
        private ClientMainGameScene _scene;
        private Microsoft.Xna.Framework.Game _monoGame;

        private Boolean _tractorBeamHeld;

        private Single _cameraLerpStrength = 0.01f;
        private Vector2 _cameraOffset;
        #endregion

        #region Protected Fields
        protected UserPlayer UserPlayer;
        #endregion

        #region Constructors
        public ClientUserPlayerDriver(Microsoft.Xna.Framework.Game monoGame, UserPlayer userPlayer, EntityInfo info, IGame game) : base(info, game)
        {
            this.UserPlayer = userPlayer;
            _monoGame = monoGame;

            _cameraOffset = Vector2.Zero;
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
                _cameraOffset = Vector2.Lerp(_cameraOffset, this.UserPlayer.Bridge.Body.LocalCenter, _cameraLerpStrength * (float)gameTime.ElapsedGameTime.TotalMilliseconds);
                _scene.Camera.Position = this.UserPlayer.Bridge.Body.Position + Vector2.Transform(_cameraOffset, this.UserPlayer.Bridge.RotationMatrix);

                // Update the local clients TractorBeam position to match the mouse
                if (_monoGame.IsActive)
                {
                    this.UserPlayer.TractorBeam.Body.Position = Vector2.Transform(ConvertUnits.ToSimUnits(Mouse.GetState().Position.ToVector2()), _scene.Camera.InverseViewMatrix);
                    _tractorBeamHeld = Mouse.GetState().RightButton == ButtonState.Pressed;
                }

                if (!this.UserPlayer.Dirty) // Mark the local player as dirty
                    this.UserPlayer.Dirty = true;
            }

            // Render the TractorBeam ConnectionNode preview, if applicable
            if(this.UserPlayer.TractorBeam.Connection != null)
            {
                // Select the closest female connection node in the current player
                var nearestFemale = this.UserPlayer.Bridge.OpenFemaleConnectionNodes()
                    .OrderBy(fn => Vector2.Distance(this.UserPlayer.TractorBeam.Body.Position, fn.WorldPoint))
                    .FirstOrDefault();

                if(nearestFemale != null && Vector2.Distance(this.UserPlayer.TractorBeam.Body.Position, nearestFemale.WorldPoint) <= TractorBeam.AttachmentDistance)
                { // If the nearest female connection node is closer than the TractorBeam.AttachmentDistance, render a preview of where the connection will take palce
                    this.UserPlayer.TractorBeam.Connection.ShipPart.Preview(nearestFemale);
                }
            }
        }
        #endregion

        #region Networking Methods (Driver Implementation)
        public override void Read(NetIncomingMessage im)
        {
            // Read the incoming TractorBeam position
            this.UserPlayer.TractorBeam.Body.Position = im.ReadVector2();
        }

        public override void Write(NetOutgoingMessage om)
        {
            // Write the current requested movement settings
            foreach (KeyValuePair<MovementType, Boolean> kvp in _requestedMovement)
            {
                om.Write((Byte)kvp.Key);
                om.Write(kvp.Value);
            }

            // Write the new TractorBeam position
            om.Write(this.UserPlayer.TractorBeam.Body.Position);
            // Write the TractorBeam held state
            om.Write(_tractorBeamHeld);
        }
        #endregion
    }
}
