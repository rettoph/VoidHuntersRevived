using Guppy;
using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.Enums;
using Guppy.Game.Common;
using Guppy.Game.Common.Enums;
using Guppy.Network;
using Guppy.Network.Identity;
using Guppy.Network.Identity.Services;
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
        public readonly IServerPeer _server;
        public readonly INetScope _scope;

        public ServerPeerComponent(IServerPeer server, INetScope scope)
        {
            _server = server;
            _scope = scope;
        }

        public void Initialize(IGuppy guppy)
        {
            _scope.AttachPeer(_server, NetScopeIds.Game);
            _server.Start(1337);

            _server.Users.OnUserConnected += HandleUserConnected;
        }

        public void Update(GameTime gameTime)
        {
            _server.Flush();
        }

        private void HandleUserConnected(IUserService sender, User args)
        {
            _server.Scopes[NetScopeIds.Game].Users.Add(args);
        }
    }
}
