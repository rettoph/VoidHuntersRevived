using Guppy.DependencyInjection;
using Guppy.Network;
using Guppy.Network.Peers;
using Guppy.UI.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Library.Scenes;
using Guppy.Extensions.DependencyInjection;
using VoidHuntersRevived.Library.Utilities;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Client.Library.Services;
using Guppy;
using Microsoft.Xna.Framework.Input;

namespace VoidHuntersRevived.Client.Library
{
    public sealed class ClientVoidHuntersRevivedGame : VoidHuntersRevivedGame
    {
        #region Private Fields
        private ClientPeer _client;
        private KeyService _keys;
        private DebugService _debug;
        private Boolean _renderDebug;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _keys);
            provider.Service(out _debug);

            _client = provider.GetService<ClientPeer>();
            _client.TryStart();

            var user = provider.GetService<User>();
            user.Name = "Rettoph";

            _client.TryConnect("localhost", 1337, user);
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            this.Scenes.Create<GameScene>();

            // Start the key service...
            _keys.TryStart();
            _keys[Keys.F3].OnKeyPressed += (k) => _renderDebug = !_renderDebug;
        }
        #endregion

        #region Frame Methods
        protected override void PostDraw(GameTime gameTime)
        {
            base.PostDraw(gameTime);

            if(_renderDebug)
                _debug.TryDraw(gameTime);
        }

        protected override void PostUpdate(GameTime gameTime)
        {
            base.PostUpdate(gameTime);

            _debug.TryUpdate(gameTime);
        }
        #endregion
    }
}
