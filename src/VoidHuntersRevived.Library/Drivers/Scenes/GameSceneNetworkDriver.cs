using Guppy;
using Guppy.DependencyInjection;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Library.Utilities;
using Guppy.Extensions.DependencyInjection;
using Guppy.IO;
using log4net;

namespace VoidHuntersRevived.Library.Drivers.Scenes
{
    /// <summary>
    /// Primary driver that will manage recieved messages from a full
    /// authority peer (such as create and update);
    /// do nothing.
    /// </summary>
    internal class GameSceneNetworkDriver : BaseAuthorizationDriver<GameScene>
    {
        #region Private Fields
        private Queue<NetIncomingMessage> _updates;
        private ILog _logger;
        #endregion

        #region Protected Fields
        protected IEnumerable<NetIncomingMessage> updates => _updates;
        #endregion

        #region Lifecycle Methods
        protected void ConfigureBase(ServiceProvider provider)
        {
            _updates = new Queue<NetIncomingMessage>();

            provider.Service(out _logger);

            this.driven.Group.Messages.Set("entity:update", this.HandleEntityUpdateMessage);
        }

        protected void DisposeBase()
        {
            this.driven.Group.Messages.Remove("entity:update");
        }
        #endregion

        #region Frame Methods
        protected virtual void Update(GameTime gameTime)
        {
            var u = _updates.Count;
            while (_updates.Any())
                this.UpdateNetworkEntity(_updates.Dequeue());
        }
        #endregion

        #region Network Entity Manipulation Methods
        private void UpdateNetworkEntity(NetIncomingMessage im)
        {
            try
            {
                this.driven.Entities.GetById<NetworkEntity>(im.ReadGuid()).TryRead(im);
            }
            catch(NullReferenceException e)
            {
                im.Position -= 128;

                _logger.Warn($"Unable to update NetworkEntity({im.ReadGuid()})");
            }
        }
        #endregion

        #region Message Handlers
        private Boolean HandleEntityUpdateMessage(NetIncomingMessage im)
        {
            _updates.Enqueue(im);
            return false;
        }
        #endregion
    }
}
