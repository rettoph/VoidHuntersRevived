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

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// Represents an entity that will automatically
    /// be stransmited through the network & has the ability
    /// to be updated.
    /// </summary>
    public class NetworkEntity : Entity, INetworkService
    {
        #region Private Fields
        private GameScene _scene;
        private GameAuthorization _authorization;
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

        /// <summary>
        /// The current object game authroization status. By default, this is the global
        /// GameAuthorization value but it may be over written.
        /// </summary>
        public GameAuthorization Authorization
        {
            get => _authorization;
            set
            {
                if(value != _authorization)
                {
                    if (this.OnAuthorizationChanged == null)
                        _authorization = value;
                    else
                        this.OnAuthorizationChanged.Invoke(this, _authorization, _authorization = value);
                }
                    
            }
        }
        #endregion

        #region Events
        public event NetIncomingMessageDelegate OnRead;
        public event NetOutgoingMessageDelegate OnWrite;
        public event GuppyDeltaEventHandler<NetworkEntity, GameAuthorization> OnAuthorizationChanged;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _scene);

            this.settings = provider.GetService<Settings>();
            this.log = provider.GetService<ILog>();

            // Create and setup a brand new action delegater instance...
            this.Actions = new MessageManager(this.BuildActionMessage);
            this.Authorization = this.settings.Get<GameAuthorization>();
        }

        protected override void PostInitialize(ServiceProvider provider)
        {
            base.PostInitialize(provider);

            this.log.Verbose(() => $"Created new NetworkEntity<{this.GetType().Name}>({this.Id}) => '{this.ServiceDescriptor.Name}'");
        }
        #endregion

        #region Frame Methods
        protected override void PostUpdate(GameTime gameTime)
        {
            base.PostUpdate(gameTime);
        }
        #endregion

        #region INetworkService Implementation
        public void TryRead(NetIncomingMessage im)
        {
            this.Actions.Read(im);
            // Read actions
            this.OnRead?.Invoke(im);
        }

        public void TryWrite(NetOutgoingMessage om)
        {
            // Write actions
            this.OnWrite?.Invoke(om);
        }
        #endregion

        #region Network Methods
        private NetOutgoingMessage BuildActionMessage(NetDeliveryMethod method, Int32 sequenceChannel, NetConnection recipient = null)
            => NetworkEntityMessageBuilder.BuildUpdateMessage(method, sequenceChannel, _scene.Group, this, recipient);
        #endregion
    }
}
