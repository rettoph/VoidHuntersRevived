using Guppy.DependencyInjection;
using Guppy.Network;
using Guppy.Network.Peers;
using Guppy.UI.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Client.Library
{
    public sealed class ClientVoidHuntersRevivedGame : VoidHuntersRevivedGame
    {
        #region Private Fields
        private ClientPeer _client;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

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
        }
        #endregion
    }
}
