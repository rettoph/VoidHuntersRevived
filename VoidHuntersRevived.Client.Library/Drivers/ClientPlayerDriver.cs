using Guppy;
using Guppy.Collections;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Client.Library.Drivers
{
    public class ClientPlayerDriver : Driver
    {
        private Player _player;
        private EntityCollection _entities;

        public ClientPlayerDriver(Player entity, EntityCollection entities, ILogger logger) : base(entity, logger)
        {
            _player = entity;
            _entities = entities;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _player.ActionHandlers.Add("update:bridge", this.HandleUpdateBrigeAction);
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
        private void HandleUpdateBrigeAction(NetIncomingMessage obj)
        {
            _player.UpdateBridge(
                _entities.GetById(obj.ReadGuid()) as FarseerEntity);
        }
        #endregion
    }
}
