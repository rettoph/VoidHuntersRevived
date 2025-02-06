using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using Guppy.Core.Network.Common;
using Guppy.Core.Network.Common.Claims;
using Guppy.Core.Network.Common.Peers;
using Guppy.Core.Network.Common.Services;
using Guppy.Game.Common.Enums;
using Guppy.Game.Common.Systems;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Game.Server.Components.Scene
{
    public class ServerPeerSystem(IServerPeer server, INetScope<IStrategy> scope) : ISceneSystem<ServerGameScene>, IUpdateSystem
    {
        private readonly IServerPeer _server = server;
        private readonly INetScope<IStrategy> _scope = scope;

        [SequenceGroup<InitializeSequenceGroupEnum>(InitializeSequenceGroupEnum.Setup)]
        public void Initialize(ServerGameScene scene)
        {
            this._server.Start(1337, Claim.Public("username", "System"));
            this._server.Users.OnUserConnected += this.HandleUserConnected;
        }

        [SequenceGroup<UpdateSequenceGroupEnum>(UpdateSequenceGroupEnum.PostUpdate)]
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