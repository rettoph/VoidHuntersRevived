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
using VoidHuntersRevived.Library.Entities.Aether;

namespace VoidHuntersRevived.Library.Scenes
{
    public class PrimaryScene : NetworkScene
    {
        #region Private Fields
        private AetherWorld _world;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.Layers.Create<Layer>((l, p, c) => l.SetContext(Constants.LayersContexts.Chunks));
            this.Layers.Create<Layer>((l, p, c) => l.SetContext(Constants.LayersContexts.Players));
            this.Layers.Create<Layer>((l, p, c) => l.SetContext(Constants.LayersContexts.Chains));
        }

        protected override void Initialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);

            provider.Service(out _world);
        }

        protected override void Release()
        {
            base.Release();

            _world.TryRelease();
            _world = default;
        }
        #endregion

        #region Frame Methods
        protected override void PostUpdate(GameTime gameTime)
        {
            base.PostUpdate(gameTime);

            _world.TryUpdate(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        #endregion

        #region NetworkScene Implementation
        protected override IChannel GetChannel(Peer peer)
            => peer.Channels.GetById(Constants.Channels.MainChannel);
        #endregion
    }
}
