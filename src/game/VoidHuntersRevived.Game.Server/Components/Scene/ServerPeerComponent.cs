using Guppy.Core.Common.Attributes;
using Guppy.Core.Network.Common;
using Guppy.Core.Network.Common.Claims;
using Guppy.Core.Network.Common.Peers;
using Guppy.Core.Network.Common.Services;
using Guppy.Engine.Common.Enums;
using Guppy.Game.Common.Components;
using Guppy.Game.Common.Enums;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Game.Server.Components.Scene
{
    internal class ServerPeerComponent(IServerPeer server, INetScope<IStrategy> scope) : ISceneComponent<ServerGameScene>, IUpdatableComponent
    {
        private readonly IServerPeer _server = server;
        private readonly INetScope<IStrategy> _scope = scope;

        [SequenceGroup<InitializeComponentSequenceGroupEnum>(InitializeComponentSequenceGroupEnum.Setup)]
        public void Initialize(ServerGameScene scene)
        {
            this._server.Start(1337, Claim.Public("username", "System"));
            this._server.Users.OnUserConnected += this.HandleUserConnected;
        }

        [SequenceGroup<UpdateComponentSequenceGroupEnum>(UpdateComponentSequenceGroupEnum.PostUpdate)]
        public void Update(GameTime gameTime)
        {
            this._server.Flush();
        }

        private void HandleUserConnected(IUserService sender, IUser args)
        {
            this._scope.Group.Users.Add(args);
        }
    }
}