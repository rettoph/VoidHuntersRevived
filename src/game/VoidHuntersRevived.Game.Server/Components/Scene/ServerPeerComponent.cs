using Guppy.Core.Common.Attributes;
using Guppy.Core.Network.Common;
using Guppy.Core.Network.Common.Claims;
using Guppy.Core.Network.Common.Peers;
using Guppy.Core.Network.Common.Services;
using Guppy.Engine.Common.Enums;
using Guppy.Game.Common;
using Guppy.Game.Common.Attributes;
using Guppy.Game.Common.Components;
using Guppy.Game.Common.Enums;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Game.Server.Components.Scene
{
    [AutoLoad]
    [SceneFilter<ServerGameScene>]
    internal class ServerPeerComponent : ISceneComponent<IScene>, IUpdatableComponent
    {
        private readonly IServerPeer _server;
        private readonly INetScope<IStrategy> _scope;

        public ServerPeerComponent(IServerPeer server, INetScope<IStrategy> scope)
        {
            _server = server;
            _scope = scope;
        }

        [SequenceGroup<InitializeComponentSequenceGroup>(InitializeComponentSequenceGroup.Setup)]
        public void Initialize(IScene scene)
        {
            _server.Start(1337, Claim.Public("username", "System"));
            _server.Users.OnUserConnected += HandleUserConnected;
        }

        [SequenceGroup<UpdateComponentSequenceGroup>(UpdateComponentSequenceGroup.PostUpdate)]
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
