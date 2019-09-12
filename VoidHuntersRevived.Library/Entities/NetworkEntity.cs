using GalacticFighters.Library.Scenes;
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
using GalacticFighters.Library.Utilities.Delegater;
using Microsoft.Extensions.Logging;
using Guppy.Utilities.Options;

namespace GalacticFighters.Library.Entities
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

            this.Events.Register<NetIncomingMessage>("on:read");
            this.Events.Register<NetOutgoingMessage>("on:write");
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

            this.Events.TryInvoke<NetIncomingMessage>(this, "on:read", im);
        }

        public void TryWrite(NetOutgoingMessage om)
        {
            om.Write(this.Id);
            this.Write(om);

            this.Events.TryInvoke<NetOutgoingMessage>(this, "on:write", om);
        }
        #endregion
    }
}
