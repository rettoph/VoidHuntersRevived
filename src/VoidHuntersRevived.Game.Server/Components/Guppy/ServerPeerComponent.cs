using Guppy;
using Guppy.Attributes;
using Guppy.Network.Peers;
using Guppy.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Constants;
using Guppy.Game;
using Microsoft.Xna.Framework;
using Guppy.Common.Attributes;
using VoidHuntersRevived.Game.Server;
using Guppy.Network.Identity.Providers;
using Guppy.Network.Identity;
using Guppy.Enums;
using Guppy.Game.Common.Enums;
using Guppy.Game.Common;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations;

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
