using Guppy;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Guppy.Network.Peers;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Server.Drivers
{
    public class ServerPlayerDriver : Driver
    {
        private Player _player;
        private VoidHuntersWorldScene _scene;

        public ServerPlayerDriver(Player entity, VoidHuntersWorldScene scene, ILogger logger) : base(entity, logger)
        {
            _player = entity;
            _scene = scene;
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            // Bind action handlers
            _player.ActionHandlers.Add("update:direction", this.HandleSetDirectionAction);

            // Bind event handlers
            _player.OnDirectionUpdated += this.HandleDirectionUpdated;
        }

        public override void Draw(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        #region Action Handlers 
        private void HandleSetDirectionAction(NetIncomingMessage obj)
        {
            if (_scene.Group.Users.GetByNetConnection(obj.SenderConnection) == _player.User)
            { // If the action request came from the user who owns the player...
                _player.UpdateDirection(
                    (Direction)obj.ReadByte(),
                    obj.ReadBoolean());
            }
            else
            { // Invalid message recieved. Ban them!
                obj.SenderConnection.Disconnect("Goodbye.");
            }
        }
        #endregion

        #region Event Handlers
        private void HandleDirectionUpdated(object sender, Direction direction)
        {
            // Broadcast a new message to all connected clients...
            var action = _player.CreateActionMessage("update:direction");
            action.Write((Byte)direction);
            action.Write(_player.GetDirection(direction));
        }
        #endregion
    }
}
