using VoidHuntersRevived.Library.Scenes;
// using GalacticFighters.Library.Utilities;
using Guppy;
using Guppy.Network.Groups;
using Guppy.Network.Interfaces;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Collections;
using Microsoft.Extensions.DependencyInjection;
using VoidHuntersRevived.Library.Utilities.Delegater;
using Microsoft.Extensions.Logging;
using Guppy.Utilities.Options;

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// Represents an entity that exists
    /// on all peers and can excecute
    /// actions on an as needed basis.
    /// </summary>
    public class NetworkEntity : Entity, INetworkObject
    {
        #region Protected Fields
        protected EntityCollection entities { get; private set; }
        #endregion

        #region Public Fields
        public ActionDelegater Actions { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            this.entities = provider.GetRequiredService<EntityCollection>();
            this.Actions = ActivatorUtilities.CreateInstance<ActionDelegater>(provider, this);

            this.Events.Register<NetIncomingMessage>("read");
            this.Events.Register<NetOutgoingMessage>("write");
        }

        public override void Dispose()
        {
            base.Dispose();

            this.Actions.Dispose();
        }
        #endregion

        #region INetwork Object Implementation
        protected virtual void Read(NetIncomingMessage im)
        {
            //
        }

        protected virtual void Write(NetOutgoingMessage om)
        {
            //
        }

        public void TryRead(NetIncomingMessage im)
        {
            this.Read(im);

            this.Events.TryInvoke<NetIncomingMessage>(this, "read", im);
        }

        public void TryWrite(NetOutgoingMessage om)
        {
            om.Write(this.Id);
            this.Write(om);

            this.Events.TryInvoke<NetOutgoingMessage>(this, "write", om);
        }

        /// <summary>
        /// Write setup data. This is written only on creation. 
        /// </summary>
        /// <param name="om"></param>
        protected virtual void WritePreInitialize(NetOutgoingMessage om)
        {
            //
        }

        /// <summary>
        /// Read setup data. This is only invoked on the
        /// factory setup method.
        /// </summary>
        /// <param name="im"></param>
        protected virtual void ReadPreInitialize(NetIncomingMessage im)
        {
            //
        }

        public void TryWritePreInitialize(NetOutgoingMessage om)
        {
            this.WritePreInitialize(om);
        }

        public void TryReadPreInitialize(NetIncomingMessage im)
        {
            this.ReadPreInitialize(im);
        }

        /// <summary>
        /// Write post-initialize data. This is written only on creation. 
        /// </summary>
        /// <param name="om"></param>
        protected virtual void WritePostInitialize(NetOutgoingMessage om)
        {
            //
        }

        /// <summary>
        /// Read post-initialize data. This is only invoked on creation via
        /// the ClientNetworkSceneDriver
        /// </summary>
        /// <param name="im"></param>
        protected virtual void ReadPostInitialize(NetIncomingMessage im)
        {
            //
        }

        public void TryWritePostInitialize(NetOutgoingMessage om)
        {
            this.WritePostInitialize(om);
        }

        public void TryReadPostInitialize(NetIncomingMessage im)
        {
            this.ReadPostInitialize(im);
        }
        #endregion
    }
}
