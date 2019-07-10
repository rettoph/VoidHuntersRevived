using Guppy;
using Guppy.Collections;
using Guppy.Implementations;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Peers;
using Guppy.Network.Security;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Entities;
using VoidHuntersRevived.Client.Library.Scenes;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Client.Library.Drivers
{
    public class ClientPlayerDriver : Driver
    {
        private Player _player;
        private Pointer _pointer;
        private ClientPeer _client;
        private EntityCollection _entities;
        private FarseerCamera2D _camera;
        private VoidHuntersClientWorldScene _scene;
        private Vector2 _localCenter;

        public ClientPlayerDriver(VoidHuntersClientWorldScene scene, Player entity, Pointer pointer, ClientPeer client, FarseerCamera2D camera, EntityCollection entities, IServiceProvider provider) : base(entity, provider)
        {
            _scene = scene;
            _player = entity;
            _pointer = pointer;
            _client = client;
            _camera = camera;
            _entities = entities;
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            // Bind action handlers
            _player.ActionHandlers.Add("update:direction", this.HandleUpdateDirectionAction);

            _player.OnUserUpdated += this.HandleUserUpdated;
        }

        protected override void draw(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        protected override void update(GameTime gameTime)
        {
            if(_client.CurrentUser == _player.User)
            {
                var kState = Keyboard.GetState();

                Boolean keyDown;

                if (_scene.Chat.Typing)
                { // If the player is typing we must stop all user actions...
                    if (_player.GetDirection(Direction.Forward))
                        this.UpdateLocalDirection(Direction.Forward, false);
                    if (_player.GetDirection(Direction.TurnLeft))
                        this.UpdateLocalDirection(Direction.TurnLeft, false);
                    if (_player.GetDirection(Direction.Backward))
                        this.UpdateLocalDirection(Direction.Backward, false);
                    if (_player.GetDirection(Direction.TurnRight))
                        this.UpdateLocalDirection(Direction.TurnRight, false);
                }
                else
                { // No typing
                    if ((keyDown = kState.IsKeyDown(Keys.W)) != _player.GetDirection(Direction.Forward))
                        this.UpdateLocalDirection(Direction.Forward, keyDown);
                    if ((keyDown = kState.IsKeyDown(Keys.A)) != _player.GetDirection(Direction.TurnLeft))
                        this.UpdateLocalDirection(Direction.TurnLeft, keyDown);
                    if ((keyDown = kState.IsKeyDown(Keys.S)) != _player.GetDirection(Direction.Backward))
                        this.UpdateLocalDirection(Direction.Backward, keyDown);
                    if ((keyDown = kState.IsKeyDown(Keys.D)) != _player.GetDirection(Direction.TurnRight))
                        this.UpdateLocalDirection(Direction.TurnRight, keyDown);

                    if (_player.TractorBeam != null)
                    {
                        var curOffset = _pointer.Position - _player.Bridge.WorldCenter;

                        if (_player.TractorBeam.Offset != curOffset)
                        {
                            _player.TractorBeam.SetOffset(curOffset);
                        }
                    }
                }

                // Update camera position
                if (_player.Bridge != null)
                {
                    _localCenter = Vector2.Lerp(_localCenter, _player.Bridge.LocalCenter, 0.1f);

                    _camera.MoveTo(_player.Bridge.Position + Vector2.Transform(_localCenter, Matrix.CreateRotationZ(_player.Bridge.Rotation)));
                }
                    
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

        #region Event Handlers
        private void HandleUserUpdated(object sender, User e)
        {
            if (_client.CurrentUser == _player.User)
            {
                _pointer.OnSecondaryChanged += this.HandlePointerSecondaryChanged;
                _pointer.OnPointerLocalMovementEnded += this.HandlePointerLocalMovementEnded;
            }
            else
            {
                _pointer.OnSecondaryChanged -= this.HandlePointerSecondaryChanged;
                _pointer.OnPointerLocalMovementEnded -= this.HandlePointerLocalMovementEnded;
            }
        }

        private void HandlePointerSecondaryChanged(object sender, bool e)
        {
            

            if (e)
            {
                _player.TractorBeam.Select();

                if (_player.TractorBeam.Selected != null)
                {
                    var action = _player.CreateActionMessage("update:tractor-beam:select");
                    action.Write(_player.TractorBeam.Offset);
                    action.Write(e);
                    action.Write(_player.TractorBeam.Selected.Id);
                }
            }
            else
            {
                _player.TractorBeam.Release();

                var action = _player.CreateActionMessage("update:tractor-beam:select");
                action.Write(_player.TractorBeam.Offset);
                action.Write(e);
            }
        }

        private void HandlePointerLocalMovementEnded(object sender, Vector2 e)
        {
            // update the server alerting them of the new tractor beam position...
            var action = _player.CreateActionMessage("update:tractor-beam:offset");
            action.Write(_player.TractorBeam.Offset);
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
