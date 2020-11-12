using Guppy;
using Guppy.DependencyInjection;
using Guppy.Network.Interfaces;
using Guppy.Network.Utilities;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Utilities;
using Microsoft.Xna.Framework;
using Guppy.Network;
using Guppy.Interfaces;
using Guppy.Network.Utilities.Messages;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Library.Enums;
using Guppy.Extensions.DependencyInjection;
using Guppy.IO;
using log4net;
using Guppy.IO.Extensions.log4net;
using Guppy.Events.Delegates;
using Guppy.Utilities;
using Guppy.Extensions.System;

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// Represents an entity that will automatically
    /// be stransmited through the network & has the ability
    /// to be updated.
    /// </summary>
    public class NetworkEntity : Entity
    {
        #region Private Fields
        private GameScene _scene;
        #endregion

        #region Protected Attributes
        /// <summary>
        /// Simple reference to the global game settings.
        /// </summary>
        protected Settings settings { get; private set; }
        protected ILog log { get; private set; }
        #endregion

        #region Public Attributes
        public MessageManager Actions { get; private set; }
        #endregion

        #region Events
        public Dictionary<MessageType, NetworkEntityMessageTypeHandler> MessageHandlers { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void Create(ServiceProvider provider)
        {
            base.Create(provider);

            // Create and setup a brand new action delegater instance...
            this.MessageHandlers = DictionaryHelper.BuildEnumDictionary<MessageType, NetworkEntityMessageTypeHandler>(t => new NetworkEntityMessageTypeHandler(t, this));
            this.Actions = new MessageManager(this.BuildActionMessage);

            this.MessageHandlers[MessageType.Create].OnWrite += om => om.Write(this.ServiceConfiguration.Id);
            this.MessageHandlers[MessageType.Action].OnRead += im => this.Actions.Read(im);
        }

        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _scene);

            this.settings = provider.GetService<Settings>();
            this.log = provider.GetService<ILog>();

            this.log.Verbose(() => $"Creating new NetworkEntity<{this.GetType().Name}>({this.Id}) => '{this.ServiceConfiguration.Name}'");
        }
        #endregion

        #region Frame Methods
        protected override void PostUpdate(GameTime gameTime)
        {
            base.PostUpdate(gameTime);
        }
        #endregion

        #region Network Methods
        private NetOutgoingMessage BuildActionMessage(NetDeliveryMethod method, Int32 sequenceChannel, NetConnection recipient = null)
            => _scene.Group.Messages.Create(method, sequenceChannel, recipient).Then(om =>
            {
                this.MessageHandlers[MessageType.Action].TryWrite(om);
            });
        #endregion
    }
}
