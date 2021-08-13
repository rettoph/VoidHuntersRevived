using Guppy.DependencyInjection;
using Guppy.Network.Interfaces;
using Guppy.Network.Peers;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library;
using Guppy.Extensions.log4net;
using Microsoft.Xna.Framework;
using Guppy.Lists.Interfaces;

namespace VoidHuntersRevived.Server
{
    public class ServerPrimaryGame : PrimaryGame
    {
        #region Public Properties
        public ServerPeer Server { get; private set; }
        public override IPeer Peer => this.Server;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.Server = provider.GetService<ServerPeer>();

            this.Server.ValidateConnectionRequest += this.HandleValidateConnectionRequest;
        }
        #endregion

        #region Event Handlers
        private bool HandleValidateConnectionRequest(NetIncomingMessage sender, IUser args)
        {
            return true;
        }
        #endregion
    }
}
