using Game = VoidHuntersRevived.Core.Implementations.Game;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Networking.Interfaces;

namespace VoidHuntersRevived.Networking
{
    public class NetworkGame : Game, INetworkGame
    {
        public IPeer Peer { get; protected set; }

        public NetworkGame(ILogger logger, GraphicsDeviceManager graphics = null, ContentManager content = null, GameWindow window = null, IServiceCollection services = null) : base(logger, graphics, content, window, services)
        {
        }

        protected override void PostInitialize()
        {
            base.PostInitialize();

            if (this.Peer == null)
                this.Logger.LogError($"No INetworkGame.Peer defined at post initialization time. Please ensure this value is set.");
            else
                this.Peer.Start();
        }
    }
}
