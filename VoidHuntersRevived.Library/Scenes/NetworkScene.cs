using Guppy;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Groups;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Extensions.Collections.Concurrent;

namespace VoidHuntersRevived.Library.Scenes
{
    /// <summary>
    /// NetworkScene's are scenes that will
    /// automatically track NetworkEntity instances
    /// and send server Create, Update, & Remove
    /// messages when neccessary. These actions
    /// are implemented via custom drivers found
    /// within the Client and Server projects
    /// respectively.
    /// </summary>
    public class NetworkScene : Scene
    {
        #region Private Fields
        /// <summary>
        /// A queue of all unhandled recieved actions.
        /// </summary>
        private ConcurrentQueue<NetIncomingMessage> _actions;
        private NetIncomingMessage _im;
        #endregion

        #region Pritected Properties
        protected Double actionCount { get; private set; }
        #endregion

        #region Internal Properties
        #endregion

        #region Public Properties
        public Group Group { get; set; }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _actions = new ConcurrentQueue<NetIncomingMessage>();
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.actionCount = 0;

            this.Group.Messages.TryAdd("entity:action", this.HandleNetworkEntityActionMessage);
        }

        public override void Dispose()
        {
            base.Dispose();

            _actions.Clear();

            this.Group.Messages.TryRemove("entity:action", this.HandleNetworkEntityActionMessage);
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            // Read all new group messages at this time.
            this.Group.Messages.ReadAll();

            base.Update(gameTime);

            // Parse any new messages
            while (_actions.Any())
                if (_actions.TryDequeue(out _im))
                    this.entities.GetById<NetworkEntity>(_im.ReadGuid())?.Actions.TryInvoke(this, _im.ReadUInt32(), _im);

            // Send all group messages at this time.
            this.Group.Messages.SendAll();
        }
        #endregion

        #region Message Handlers
        private void HandleNetworkEntityActionMessage(object sender, NetIncomingMessage arg)
        {
            this.actionCount++;
            _actions.Enqueue(arg);
        }
        #endregion
    }
}
