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
using Guppy.Extensions.log4net;
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
        #endregion

        #region Public Attributes
        public MessageManager Ping { get; private set; }

        /// <summary>
        /// Indicates whether or not the current entity should broadcast
        /// an Update message through the peer. When true, the entity will
        /// be enqueued within the GameScene.dirtyEntities queue to be 
        /// sent on the next flush.
        /// </summary>
        public DirtyState DirtyState { get; set; }
        #endregion

        #region Events
        public Dictionary<MessageType, NetworkEntityMessageTypeHandler> MessageHandlers { get; private set; }

        /// <summary>
        /// Custom object used to determin whether or not a network object
        /// should be cleaned this frame.
        /// </summary>
        protected internal ValidateEventDelegate<NetworkEntity, GameTime> ValidateCleaning;
        #endregion

        #region Lifecycle Methods
        protected override void Create(ServiceProvider provider)
        {
            base.Create(provider);

            // Create and setup a brand new action delegater instance...
            this.MessageHandlers = DictionaryHelper.BuildEnumDictionary<MessageType, NetworkEntityMessageTypeHandler>(t => new NetworkEntityMessageTypeHandler(t, this));
            this.Ping = new MessageManager(this.BuildActionMessage);

            this.MessageHandlers[MessageType.Create].OnWrite += om => om.Write(this.ServiceConfiguration.Id);
            this.MessageHandlers[MessageType.Ping].OnRead += im => this.Ping.Read(im);
            this.MessageHandlers[MessageType.Update].OnWrite += im => this.DirtyState &= ~DirtyState.Cleaning;
        }

        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _scene);

            this.settings = provider.GetService<Settings>();
            this.DirtyState = DirtyState.None;

            if (this.settings.Get<HostType>() == HostType.Remote)
                this.OnPostUpdate += this.PostUpdateRemote;
        }

        protected override void Release()
        {
            base.Release();

            _scene = null;

            this.OnPostUpdate -= this.PostUpdateRemote;
        }
        #endregion

        #region Frame Methods
        private void PostUpdateRemote(GameTime gameTime)
        {
            if ((this.DirtyState & DirtyState.Cleaning) == 0 && (this.DirtyState & DirtyState.DirtyAndFilthy) != 0 && this.ValidateCleaning.Validate(this, gameTime, true))
            { // Enque the current entity to be cleaned & update the DirtyState...
                _scene.dirtyEntities.Enqueue(this);

                this.DirtyState |= DirtyState.Cleaning;
                this.DirtyState &= ~DirtyState.Dirty;
            }
        }

        #endregion

        #region Network Methods
        private NetOutgoingMessage BuildActionMessage(NetDeliveryMethod method, Int32 sequenceChannel, NetConnection recipient = null)
            => _scene.Group.Messages.Create(method, sequenceChannel, recipient).Then(om =>
            {
                this.MessageHandlers[MessageType.Ping].TryWrite(om);
            });
        #endregion
    }
}
