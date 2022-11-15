using Guppy.Common;
using Guppy.MonoGame.Services;
using Guppy.Network.Identity;
using Guppy.Network.Identity.Providers;
using Guppy.Network.Messages;
using Guppy.Network.Peers;
using Guppy.Providers;
using Guppy.Resources;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Server.Constants;

namespace VoidHuntersRevived.Server
{
    internal sealed class ServerMainGuppy : MainGuppy
    {
        private ISetting<int> _port;

        private ServerPeer _server;
        private IGuppyProvider _guppies;
        private IScoped<GameGuppy> _game;

        public ServerMainGuppy(
            IGuppyProvider guppies,
            ISettingProvider settings,
            IGameComponentService components,
            ServerPeer server) : base(server, components)
        {
            _guppies = guppies;

            _port = settings.Get<int>(SettingConstants.Port);

            _server = server;

            _server.Start(_port.Value);

            _game = _guppies.Create<ServerGameGuppy>();

            _server.Users.OnUserConnected += this.HandleUserConnected;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _game.Instance.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        private void HandleUserConnected(IUserProvider sender, User args)
        { // TODO: This will cause a race condition once multi-threading is introduced
            _game.Instance.NetScope.Users.Add(args);
        }
    }
}
