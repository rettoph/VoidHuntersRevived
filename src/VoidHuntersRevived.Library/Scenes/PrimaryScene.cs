using Guppy;
using Guppy.EntityComponent.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Players;
using Microsoft.Xna.Framework;
using Guppy.Network.Interfaces;
using Guppy.Utilities;
using VoidHuntersRevived.Library.Entities.Aether;
using VoidHuntersRevived.Library.Globals.Constants;
using Guppy.Network;
using Guppy.Network.Services;
using Guppy.Threading.Utilities;

namespace VoidHuntersRevived.Library.Scenes
{
    public class PrimaryScene : Scene
    {
        #region Private Fields
        private AetherWorld _world;
        private NetworkEntityService _networkEntities;
        private MessageBus _bus;
        #endregion

        #region Public Properties
        public Room Room { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.Room = provider.GetService<Peer>().Rooms.GetById(0);
            this.Room.TryBindToScope(provider);

            this.Layers.Create<Layer>((l, p, c) => l.SetContext(LayersContexts.Chunks));
            this.Layers.Create<Layer>((l, p, c) => l.SetContext(LayersContexts.Players));
            this.Layers.Create<Layer>((l, p, c) => l.SetContext(LayersContexts.Ships));
            this.Layers.Create<Layer>((l, p, c) => l.SetContext(LayersContexts.Chains));

            provider.Service(out _world);
            provider.Service(out _networkEntities);
            provider.Service(out _bus);
        }

        protected override void PostUninitialize()
        {
            base.PostUninitialize();

            _world.Dispose();

            this.Room.TryUnbindToScope();
        }
        #endregion

        #region Frame Methods
        protected override void PreUpdate(GameTime gameTime)
        {
            base.PreUpdate(gameTime);
        }

        protected override void PostUpdate(GameTime gameTime)
        {
            base.PostUpdate(gameTime);

            _world.TryUpdate(gameTime);
        }
        #endregion
    }
}
