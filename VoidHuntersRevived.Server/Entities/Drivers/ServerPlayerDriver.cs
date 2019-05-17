﻿using System;
using System.Collections.Generic;
using System.Text;
using Guppy;
using Guppy.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Library.Entities.Drivers;
using VoidHuntersRevived.Library.Entities.Players;
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Library.Enums;
using Lidgren.Network;
using Guppy.Network.Groups;
using Guppy.Network.Peers;

namespace VoidHuntersRevived.Server.Entities.Drivers
{
    public class ServerPlayerDriver : PlayerDriver
    {
        private ServerPeer _server;
        
        private Int32 _lastPositionUpdate;
        private Int32 _lastVelocityUpdate;

        private Vector2 _oldPosition;
        private Vector2 _oldVelocity;

        private Single _oldRotation;
        private Single _oldAngularVelocity;

        private Int32 _updateInterval;

        public ServerPlayerDriver(
            Player parent, 
            EntityConfiguration configuration, 
            Scene scene, 
            ILogger logger,
            ServerPeer server) : base(parent, configuration, scene, logger)
        {
            _server = server;
            _updateInterval = 1000;

            this.parent.ActionHandlers.Add("update:direction", this.HandleUpdateDirectionAction);
        }

        public override void Draw(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            if (this.parent.Bridge.Body.Awake)
            {
                if (_oldPosition != this.parent.Bridge.Body.Position || _oldRotation != this.parent.Bridge.Body.Rotation)
                {
                    _lastPositionUpdate += (Int32)gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (_lastPositionUpdate > _updateInterval)
                        this.SendUpdatePositionAction();
                }

                if (_oldVelocity != this.parent.Bridge.Body.LinearVelocity || _oldAngularVelocity != this.parent.Bridge.Body.AngularVelocity)
                {
                    _lastVelocityUpdate += (Int32)gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (_lastVelocityUpdate > _updateInterval)
                        this.SendUpdateVelocityAction();
                }
            }
        }

        public override void HandleUpdateBridge()
        {
            var action = this.parent.CreateActionMessage("update:bridge");
            action.Write(this.parent.Bridge.Id);
        }

        public override void HandleUpdateDirection(Direction direction, bool value)
        {
            var action = this.parent.CreateActionMessage("update:direction");
            action.Write((Byte)direction);
            action.Write(value);

            // Push new update actions
            this.SendUpdatePositionAction();
            this.SendUpdateVelocityAction();
        }

        #region Action Handlers
        private void HandleUpdateDirectionAction(NetIncomingMessage obj)
        {
            var senderUser = _server.Users.GetByNetConnection(obj.SenderConnection);

            if (senderUser == this.parent.User)
            { // If the sender user matches the player user...
                this.parent.UpdateDirection(
                    (Direction)obj.ReadByte(),
                    obj.ReadBoolean());
            }
            else
            { // If the sender user does not match the player user...
                obj.SenderConnection.Disconnect("Goodbye.");
            }
        }
        #endregion

        private void SendUpdatePositionAction()
        {
            var action = this.parent.CreateActionMessage("update:position");
            action.Write(this.parent.Bridge.Body.Position);
            action.Write(this.parent.Bridge.Body.Rotation);

            _oldPosition = this.parent.Bridge.Body.Position;
            _oldRotation = this.parent.Bridge.Body.Rotation;
            _lastPositionUpdate = _lastPositionUpdate % _updateInterval;
        }

        private void SendUpdateVelocityAction()
        {
            var action = this.parent.CreateActionMessage("update:velocity");
            action.Write(this.parent.Bridge.Body.LinearVelocity);
            action.Write(this.parent.Bridge.Body.AngularVelocity);

            _oldVelocity = this.parent.Bridge.Body.LinearVelocity;
            _oldAngularVelocity = this.parent.Bridge.Body.AngularVelocity;
            _lastVelocityUpdate = _lastPositionUpdate % _updateInterval;
        }
    }
}
