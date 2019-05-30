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
using Guppy.Implementations;
using Guppy.Collections;
using VoidHuntersRevived.Library.Configurations;

namespace VoidHuntersRevived.Server.Drivers
{
    public class ServerPlayerDriver : Driver
    {
        private Player _player;
        private VoidHuntersWorldScene _scene;
        private EntityCollection _entities;

        public ServerPlayerDriver(Player entity, VoidHuntersWorldScene scene, EntityCollection entities, IServiceProvider provider, ILogger logger) : base(entity, provider, logger)
        {
            _player = entity;
            _scene = scene;
            _entities = entities;
        }

        protected override void Boot()
        {
            base.Boot();

            // Bind event handlers
            _player.OnDirectionUpdated += this.HandleDirectionUpdated;
            _player.OnInitializatingInternals += this.HandlePlayerInitializatingInternals;
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            // Bind action handlers
            _player.ActionHandlers.Add("update:direction", this.HandleSetDirectionAction);

            
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

        private void HandlePlayerInitializatingInternals(object sender, PlayerInstanceInternalsConfiguration e)
        {
            e.TractorBeam = _entities.Create<TractorBeam>("entity:tractor-beam");
        }
        #endregion
    }
}
