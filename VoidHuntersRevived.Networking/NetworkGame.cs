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

        protected override void Initialize()
        {
            base.Initialize();

            if ((this.Peer = this.Provider.GetService<IPeer>()) == null)
                this.Logger.LogError($"No IPeer service is not defined. Please ensure it is added as a service at ConfigureServices time.");
            else
                this.Peer.Start();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Update the peer
            this.Peer.Update();
        }
    }
}
