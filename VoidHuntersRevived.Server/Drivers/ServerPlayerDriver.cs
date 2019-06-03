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
        private Queue<Boolean> _tractorBeamSelectQueue;

        public ServerPlayerDriver(Player entity, VoidHuntersWorldScene scene, EntityCollection entities, IServiceProvider provider, ILogger logger) : base(entity, provider, logger)
        {
            _player = entity;
            _scene = scene;
            _entities = entities;
        }

        protected override void Boot()
        {
            base.Boot();

            _tractorBeamSelectQueue = new Queue<Boolean>();

            // Bind event handlers
            _player.OnDirectionUpdated += this.HandleDirectionUpdated;
            _player.OnInitializatingInternals += this.HandlePlayerInitializatingInternals;
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            // Bind action handlers
            _player.ActionHandlers["update:direction"] = this.HandleSetDirectionAction;
            _player.ActionHandlers["update:tractor-beam:offset"] = this.HandleUpdateTractorBeamPositionAction;
            _player.ActionHandlers["update:tractor-beam:select"] = this.HandleUpdateTractorBeamSelectAction;
        }

        public override void Draw(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            while(_tractorBeamSelectQueue.Count > 0)
            {
                if (_tractorBeamSelectQueue.Dequeue())
                    _player.TractorBeam.Select();
                else
                    _player.TractorBeam.Release();
            }
        }

        #region Action Handlers 
        private void HandleSetDirectionAction(NetIncomingMessage obj)
        {
            if (this.ValidateSender(obj))
            { // If the action request came from the user who owns the player...
                _player.UpdateDirection(
                    (Direction)obj.ReadByte(),
                    obj.ReadBoolean());
            }
        }

        private void HandleUpdateTractorBeamPositionAction(NetIncomingMessage obj)
        {
            if (this.ValidateSender(obj))
                _player.TractorBeam.SetOffset(obj.ReadVector2());
        }


        private void HandleUpdateTractorBeamSelectAction(NetIncomingMessage obj)
        {
            if (this.ValidateSender(obj))
            {
                _player.TractorBeam.SetOffset(obj.ReadVector2());

                _tractorBeamSelectQueue.Enqueue(obj.ReadBoolean());
            }
        }

        private Boolean ValidateSender(NetIncomingMessage obj)
        {
            if (_scene.Group.Users.GetByNetConnection(obj.SenderConnection) == _player.User)
            { // If the action request came from the user who owns the player...
                return true;
            }
            else
            { // Invalid message recieved. Ban them!
                obj.SenderConnection.Disconnect("Goodbye.");
                return false;
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
            e.TractorBeam = _entities.Create<TractorBeam>("entity:tractor-beam", _player);
        }
        #endregion
    }
}
