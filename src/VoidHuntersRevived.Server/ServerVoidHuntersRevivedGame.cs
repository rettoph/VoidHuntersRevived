using Guppy.DependencyInjection;
using Guppy.Network;
using Guppy.Network.Peers;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Library.Scenes;
using Guppy.Extensions.DependencyInjection;

namespace VoidHuntersRevived.Server
{
    public class ServerVoidHuntersRevivedGame : VoidHuntersRevivedGame
    {
        private ServerPeer _server;

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _server = provider.GetService<ServerPeer>();
            _server.TryStart();

            _server.Users.OnAdded += this.HandleUserJoined;
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            this.Scenes.Create<GameScene>();
        }
        #endregion

        #region Event Handlers
        private void HandleUserJoined(object sender, User e)
        {
            var group = _server.Groups.GetOrCreateById(Guid.Empty);
            group.Users.TryAdd(e);
        }
        #endregion
    }
}
