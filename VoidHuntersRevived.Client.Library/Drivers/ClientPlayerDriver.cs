﻿using Guppy;
using Guppy.Collections;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Peers;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Client.Library.Drivers
{
    public class ClientPlayerDriver : Driver
    {
        private Player _player;
        private ClientPeer _client;
        private EntityCollection _entities;
        private FarseerCamera2D _camera;

        public ClientPlayerDriver(Player entity, ClientPeer client, FarseerCamera2D camera, EntityCollection entities, ILogger logger) : base(entity, logger)
        {
            _player = entity;
            _client = client;
            _camera = camera;
            _entities = entities;
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            // Bind action handlers
            _player.ActionHandlers.Add("update:direction", this.HandleUpdateDirectionAction);
        }

        public override void Draw(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            if(_client.CurrentUser == _player.User)
            {
                var kState = Keyboard.GetState();

                Boolean keyDown;

                if((keyDown = kState.IsKeyDown(Keys.W)) != _player.GetDirection(Direction.Forward))
                    this.UpdateLocalDirection(Direction.Forward, keyDown);
                if ((keyDown = kState.IsKeyDown(Keys.A)) != _player.GetDirection(Direction.TurnLeft))
                    this.UpdateLocalDirection(Direction.TurnLeft, keyDown);
                if ((keyDown = kState.IsKeyDown(Keys.S)) != _player.GetDirection(Direction.Backward))
                    this.UpdateLocalDirection(Direction.Backward, keyDown);
                if ((keyDown = kState.IsKeyDown(Keys.D)) != _player.GetDirection(Direction.TurnRight))
                    this.UpdateLocalDirection(Direction.TurnRight, keyDown);

                if(_player.Bridge.Body != null)
                    _camera.MoveTo(_player.Bridge.Body.WorldCenter);
            }
        }

        #region Action Handlers
        private void HandleUpdateDirectionAction(NetIncomingMessage obj)
        {
            _player.UpdateDirection(
                (Direction)obj.ReadByte(),
                obj.ReadBoolean());
        }
        #endregion

        /// <summary>
        /// Sets the player direction locally,
        /// and send a message to the server
        /// alerting them of this change.
        /// </summary>
        private void UpdateLocalDirection(Direction direction, Boolean value)
        {
            _player.UpdateDirection(direction, value);

            var action = _player.CreateActionMessage("update:direction");
            action.Write((Byte)direction);
            action.Write(value);
        }
    }
}
