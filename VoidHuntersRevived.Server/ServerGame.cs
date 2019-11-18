using Guppy.Network.Peers;
using Guppy.Network.Security;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Server.Scenes;

namespace VoidHuntersRevived.Server
{
    /// <summary>
    /// The server specific version of the BaseGame class
    /// </summary>
    public class ServerGame : BaseGame
    {
        #region Private Fields
        private ServerPeer _server;
        #endregion

        #region Constructor
        public ServerGame(ServerPeer server) : base(server)
        {
            _server = server;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.scenes.Create<ServerWorldScene>(s =>
            {
                s.Group = _server.Groups.GetOrCreateById(Guid.Empty);
            }).TryStartAsync();

            // Add event handlers
            _server.Users.Events.TryAdd<User>("added", this.HandleUserJoined);
        }
        #endregion

        #region Event Handlers
        private void HandleUserJoined(object sender, User arg)
        {
            // Auto add new users to the main group
            _server.Groups.GetOrCreateById(Guid.Empty).Users.Add(arg);
        }
        #endregion
    }
}
