using Guppy;
using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Players;
using Guppy.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Guppy.Network.Interfaces;
using Guppy.Network.Scenes;
using Guppy.Network.Peers;
using Guppy.Utilities;

namespace VoidHuntersRevived.Library.Scenes
{
    public class PrimaryScene : NetworkScene
    {
        #region Lifecycle Methods
        protected override void Initialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);

            this.Layers.Create<Layer>((l, p, c) => l.SetContext(Constants.LayersContexts.Chunks));
            this.Layers.Create<Layer>((l, p, c) => l.SetContext(Constants.LayersContexts.Players));
            this.Layers.Create<Layer>((l, p, c) => l.SetContext(Constants.LayersContexts.Chains));
        }
        #endregion

        #region NetworkScene Implementation
        protected override IChannel GetChannel(Peer peer)
            => peer.Channels.GetById(Constants.Channels.MainChannel);
        #endregion
    }
}
