using Guppy;
using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.Enums;
using Guppy.Game.Common;
using Guppy.Game.Common.Enums;
using Guppy.Network;
using Guppy.Network.Identity;
using Guppy.Network.Identity.Providers;
using Guppy.Network.Peers;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Constants;

namespace VoidHuntersRevived.Game.Server.Components.Guppy
{
    [AutoLoad]
    [GuppyFilter<ServerGameGuppy>]
    [Sequence<InitializeSequence>(InitializeSequence.Setup)]
    [Sequence<UpdateSequence>(UpdateSequence.PostUpdate)]
    internal class ServerPeerComponent : IGuppyComponent, IGuppyUpdateable
    {
        public readonly ServerPeer _server;
        public readonly NetScope _scope;

        public ServerPeerComponent(ServerPeer server, NetScope scope)
        {
            _server = server;
            _scope = scope;
        }

        public void Initialize(IGuppy guppy)
        {
            _server.Bind(_scope, NetScopeIds.Game);
            _server.Start(1337);

            _server.Users.OnUserConnected += HandleUserConnected;
        }

        public void Update(GameTime gameTime)
        {
            _server.Flush();
        }

        private void HandleUserConnected(IUserProvider sender, User args)
        {
            _server.Scopes[NetScopeIds.Game].Users.Add(args);
        }
    }
}
