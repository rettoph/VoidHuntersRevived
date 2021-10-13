using Guppy.CommandLine.Services;
using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.Network.Components;
using Guppy.Network.Contexts;
using Guppy.Network.Enums;
using Guppy.Network.Interfaces;
using Guppy.Network.Utilities;
using Guppy.Threading.Utilities;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text;
using VoidHuntersRevived.Library.Entities.Players;

namespace VoidHuntersRevived.Library.Components.Entities.Players
{
    public abstract class UserPlayerCurrentUserActionBaseComponent<TAction> : UserPlayerCurrentUserBaseComponent
    {
        #region Private Fields
        private ThreadQueue _mainThread;
        #endregion

        #region Public Properties
        /// <summary>
        /// Describes the unique message identifier when a CurrentUser must make an action request.
        /// </summary>
        public abstract UInt32 ActionRequestMessageType { get; }

        /// <summary>
        /// Custom context for the <see cref="ActionRequestMessageType"/> message.
        /// </summary>
        public virtual NetOutgoingMessageContext ActionRequestMessageContext => Guppy.Network.Constants.MessageContexts.InternalReliableSecondary;
        #endregion

        #region Events
        public event OnEventDelegate<UserPlayerCurrentUserActionBaseComponent<TAction>, TAction> OnActionRequest;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _mainThread);
        }

        protected override void PreInitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.PreInitializeRemote(provider, networkAuthorization);

            this.Entity.Messages.Add(
                messageType: this.ActionRequestMessageType,
                defaultContext: this.ActionRequestMessageContext);
        }

        protected override void InitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.InitializeRemote(provider, networkAuthorization);

            if (networkAuthorization == NetworkAuthorization.Master)
                this.Entity.Messages[this.ActionRequestMessageType].OnRead += this.ReadCurrentUserMasterActionRequestMessage;
        }

        protected override void InitializeRemoteCurrentUser(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.InitializeRemoteCurrentUser(provider, networkAuthorization);

            // We only need to invoke the request network message on slaves.
            if(networkAuthorization == NetworkAuthorization.Slave)
                this.OnActionRequest += this.HandleRemoteCurrentUserSlaveActionRequest;
        }

        protected override void ReleaseRemoteCurrentUser(NetworkAuthorization networkAuthorization)
        {
            base.ReleaseRemoteCurrentUser(networkAuthorization);

            // We only need to invoke the request network message on slaves.
            if (networkAuthorization == NetworkAuthorization.Slave)
                this.OnActionRequest -= this.HandleRemoteCurrentUserSlaveActionRequest;
        }

        protected override void ReleaseRemote(NetworkAuthorization networkAuthorization)
        {
            base.ReleaseRemote(networkAuthorization);


            if (networkAuthorization == NetworkAuthorization.Master)
                this.Entity.Messages[this.ActionRequestMessageType].OnRead -= this.ReadCurrentUserMasterActionRequestMessage;
        }

        protected override void PostReleaseRemote(NetworkAuthorization networkAuthorization)
        {
            base.PostReleaseRemote(networkAuthorization);
        }

        protected override void PostRelease()
        {
            base.PostRelease();

            _mainThread = default;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Attempt to do a recieved <see cref="TAction"/> <paramref name="request"/>. This should be manually invoked, 
        /// generally in a <see cref="ICommandHandler"/>.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected void TryDoActionRequest(TAction request)
        {
            _mainThread.Enqueue(_ =>
            {
                if (this.TryDoActionRequest(request, out TAction response))
                {
                    this.OnActionRequest?.Invoke(this, response);
                }
            });
        }

        /// <summary>
        /// <para>WARNING: DO NO CALL THIS MANUALLY.</para>
        /// <para>If you think you need to call this you probably need to call <see cref="TryDoActionRequest(TAction)"/> instead.</para>
        /// 
        /// <para>Attempt to do a recieved <see cref="TAction"/> and return the result.</para>
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        protected abstract Boolean TryDoActionRequest(TAction request, out TAction response);
        #endregion

        #region Network Methods
        /// <summary>
        /// When the master recieves a <see cref="ActionRequestMessageType"/> message, this will read it and attempt
        /// to invoke the action server side.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="im"></param>
        private void ReadCurrentUserMasterActionRequestMessage(MessageTypeManager sender, NetIncomingMessage im)
        {
            // Parse incoming data...
            TAction request = this.ReadCurrentUserActionRequestMessage(im);

            // Attempt to invoke the action...
            this.TryDoActionRequest(request);

            //TODO: If TryDoActionRequest fails, we should return a response to the requester.
        }

        /// <summary>
        /// Write the recieved <see cref="TAction"/> <paramref name="request"/> to the <paramref name="om"/>.
        /// </summary>
        /// <param name="om"></param>
        /// <param name="request"></param>
        protected abstract void WriteCurrentUserActionRequestMessage(NetOutgoingMessage om, TAction request);

        /// <summary>
        /// Read and return <see cref="TAction"/> instance from the recieved <paramref name="im"/>.
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        protected abstract TAction ReadCurrentUserActionRequestMessage(NetIncomingMessage im);
        #endregion

        #region Event Handlers
        /// <summary>
        /// Automatically generate a <see cref="NetOutgoingMessage"/> instance to request
        /// the current action through the peer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void HandleRemoteCurrentUserSlaveActionRequest(UserPlayerCurrentUserActionBaseComponent<TAction> sender, TAction request)
        {
            this.Entity.Messages[this.ActionRequestMessageType].Create(om =>
            {
                this.WriteCurrentUserActionRequestMessage(om, request);
            }, this.Entity.Pipe);
        }
        #endregion
    }
}
