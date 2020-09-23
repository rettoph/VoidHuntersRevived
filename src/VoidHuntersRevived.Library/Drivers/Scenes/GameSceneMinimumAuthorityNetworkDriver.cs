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
using Guppy.IO.Extensions.log4net;
using Guppy.Network.Utilities.Messages;

namespace VoidHuntersRevived.Library.Drivers.Scenes
{
    /// <summary>
    /// Primary driver that will manage recieved messages from a full
    /// authority peer (such as create and update);
    /// do nothing.
    /// </summary>
    internal sealed class GameSceneMinimumAuthorityNetworkDriver : GameSceneNetworkDriver
    {
        #region Private Fields
        private Queue<NetIncomingMessage> _creates;
        private Queue<NetIncomingMessage> _removes;
        private ILog _logger;
        private Byte _state;
        #endregion

        #region Lifecycle Methods
        protected override void ConfigureMinimum(ServiceProvider provider)
        {
            base.ConfigureMinimum(provider);

            this.ConfigureBase(provider);

            provider.Service(out _logger);

            _creates = new Queue<NetIncomingMessage>();
            _removes = new Queue<NetIncomingMessage>();

            this.driven.Group.Messages.Set("scene:setup", this.HandleSceneSetupMessage);
            this.driven.Group.Messages.Set("entity:create", this.HandleEntityCreateMessage);
            this.driven.Group.Messages.Set("entity:remove", this.HandleEntityRemoveMessage);
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.DisposeBase();

            this.driven.Group.Messages.Remove("scene:setup");
            this.driven.Group.Messages.Remove("entity:create");
            this.driven.Group.Messages.Remove("entity:remove");
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            while (_creates.Any())
                this.CreateNetworkEntity(_creates.Dequeue());

            base.Update(gameTime);

            while (_removes.Any())
                this.RemoveNetworkEntity(_removes.Dequeue());
        }
        #endregion

        #region Network Entity Manipulation Methods
        private void CreateNetworkEntity(NetIncomingMessage im)
            => this.driven.Entities.GetOrCreateById<NetworkEntity>(im.ReadGuid(), im.ReadUInt32());

        private void SetupNetworkEntity(NetIncomingMessage im)
            => this.driven.Entities.GetById<NetworkEntity>(im.ReadGuid()).TryRead(im);

        private void RemoveNetworkEntity(NetIncomingMessage im)
            => this.driven.Entities.GetById<NetworkEntity>(im.ReadGuid()).TryRelease();
        #endregion

        #region Message Handlers
        private void HandleSceneSetupMessage(NetIncomingMessage im)
        {
            _state++;
            // If the setup is complete...
            if (im.ReadBoolean())
            {
                _logger.Info(() => $"GameScene: Setup completed.\nCreates: {_creates.Count()}\nUpdates: {this.updates.Count()}\nRemoves: {_removes.Count()}");
                this.driven.OnUpdate += this.Update;
            }
            else
            {
                _logger.Info(() => "GameScene: Setup started.");
            }
        }

        private Boolean HandleEntityCreateMessage(NetIncomingMessage im)
        {
            _creates.Enqueue(im);
            return false;
        }

        private Boolean HandleEntityRemoveMessage(NetIncomingMessage im)
        {
            _removes.Enqueue(im);
            return false;
        }
        #endregion
    }
}
