using Guppy;
using Lidgren.Network;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Utilities.Delegaters;

namespace VoidHuntersRevived.Library.Entities
{
    public class NetworkEntity : Entity
    {
        #region Public Properties
        public ActionMessageDelegater Actions { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            // Create a new action delegater
            this.Actions = provider.GetRequiredService<ActionMessageDelegater>();
        }

        public override void Dispose()
        {
            base.Dispose();

            // Dispose of the earlier delegater
            this.Actions.Dispose();
        }
        #endregion

        #region Network Methods
        /// <summary>
        /// Read setup data
        /// </summary>
        /// <param name="im"></param>
        public virtual void TryReadSetup(NetIncomingMessage im)
        {
            this.ReadSetup(im);
        }

        /// <summary>
        /// Write setup data
        /// </summary>
        /// <param name="im"></param>
        public virtual void TryWriteSetup(NetOutgoingMessage om)
        {
            this.WriteSetup(om);
        }

        /// <summary>
        /// Read the full entity network data
        /// </summary>
        /// <param name="im"></param>
        public virtual void TryRead(NetIncomingMessage im)
        {
            this.Read(im);
        }

        /// <summary>
        /// Write the full entity network data
        /// </summary>
        /// <param name="om"></param>
        public virtual void TryWrite(NetOutgoingMessage om)
        {
            this.Write(om);
        }

        /// <summary>
        /// Read setup data
        /// </summary>
        /// <param name="im"></param>
        protected virtual void ReadSetup(NetIncomingMessage im)
        {
            //
        }

        /// <summary>
        /// Write setup data
        /// </summary>
        /// <param name="im"></param>
        protected virtual void WriteSetup(NetOutgoingMessage om)
        {
            //
        }

        /// <summary>
        /// Read the full entity network data
        /// </summary>
        /// <param name="im"></param>
        protected virtual void Read(NetIncomingMessage im)
        {
            //
        }

        /// <summary>
        /// Write the full entity network data
        /// </summary>
        /// <param name="om"></param>
        protected virtual void Write(NetOutgoingMessage om)
        {
            //
        }
        #endregion
    }
}
