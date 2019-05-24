using Guppy;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using Guppy.Network.Extensions.Lidgren;

namespace VoidHuntersRevived.Server.Drivers
{
    public class ServerPlayerDriver : Driver
    {
        private Player _player;

        public ServerPlayerDriver(Player entity, ILogger logger) : base(entity, logger)
        {
            _player = entity;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _player.OnBridgeUpdated += this.HandlePlayerBridgeUpdated;
        }

        public override void Draw(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        #region Event Handlers
        private void HandlePlayerBridgeUpdated(object sender, FarseerEntity e)
        {
            // Alert all connected clients of the bridge update
            var action = _player.CreateActionMessage("update:bridge");
            action.Write(_player.Bridge.Id);
        }
        #endregion

        public override void Dispose()
        {
            base.Dispose();

            _player.OnBridgeUpdated -= this.HandlePlayerBridgeUpdated;
        }
    }
}
