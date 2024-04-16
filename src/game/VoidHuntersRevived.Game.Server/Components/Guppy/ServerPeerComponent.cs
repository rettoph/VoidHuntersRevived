using Guppy.Core.Common.Attributes;
using Guppy.Core.Network.Common;
using Guppy.Core.Network.Common.Claims;
using Guppy.Core.Network.Common.Peers;
using Guppy.Core.Network.Common.Services;
using Guppy.Engine.Common;
using Guppy.Engine.Common.Components;
using Guppy.Engine.Common.Enums;
using Guppy.Game.Common;
using Guppy.Game.Common.Enums;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Game.Server.Components.Guppy
{
    [AutoLoad]
    [GuppyFilter<ServerGameGuppy>]
    [Sequence<InitializeSequence>(InitializeSequence.Setup)]
    [Sequence<UpdateSequence>(UpdateSequence.PostUpdate)]
    internal class ServerPeerComponent : IGuppyComponent, IGuppyUpdateable
    {
        private readonly IServerPeer _server;
        private readonly INetScope<ISimulation> _scope;

        public ServerPeerComponent(IServerPeer server, INetScope<ISimulation> scope)
        {
            _server = server;
            _scope = scope;
        }

        public void Initialize(IGuppy guppy)
        {
            _server.Start(1337, Claim.Public("username", "System"));
            _server.Users.OnUserConnected += HandleUserConnected;
        }

        public void Update(GameTime gameTime)
        {
            _server.Flush();
        }

        private void HandleUserConnected(IUserService sender, IUser args)
        {
            _scope.Group.Users.Add(args);
        }
    }
}
