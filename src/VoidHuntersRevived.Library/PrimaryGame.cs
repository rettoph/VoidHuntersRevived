using Guppy;
using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Extensions.log4net;
using Guppy.Interfaces;
using Guppy.Lists;
using Guppy.Network.Interfaces;
using Guppy.Network.Peers;
using Guppy.Network.Structs;
using Guppy.Utilities;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities.Chunks;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Library
{
    public abstract class PrimaryGame : Guppy.Game
    {
        #region Public Properties
        public abstract IPeer Peer { get; }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.PreInitialize(provider);
        }

        protected override void Initialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);

            this.Peer.StartAsync(50);

            // this.Peer.DiagnosticInterval = 1000;
        }

        protected override void PostInitialize(GuppyServiceProvider provider)
        {
            base.PostInitialize(provider);

            this.Scenes.Create<PrimaryScene>();
        }
        #endregion

        #region Frame Methods
        protected override void PreUpdate(GameTime gameTime)
        {
            base.PreUpdate(gameTime);

            // this.Peer.TryUpdate();
        }
        #endregion
    }
}
