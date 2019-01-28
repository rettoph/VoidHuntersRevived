using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Core.Extensions;
using VoidHuntersRevived.Core.Providers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using VoidHuntersRevived.Core.Loaders;
using VoidHuntersRevived.Core.Factories;
using VoidHuntersRevived.Networking.Peers;
using VoidHuntersRevived.Networking.Interfaces;
using Lidgren.Network;
using VoidHuntersRevived.Server.Scenes;

namespace VoidHuntersRevived.Server
{
    class VoidHuntersRevivedServerGame : VoidHuntersRevivedGame
    {
        public VoidHuntersRevivedServerGame(ILogger logger, GraphicsDeviceManager graphics = null, ContentManager content = null, GameWindow window = null, IServiceCollection services = null) : base(logger, graphics, content, window, services)
        {
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.AddSingleton<IPeer>(new ServerPeer("vhr", 1337, this, this.Logger));
        }

        protected override void Initialize()
        {
            base.Initialize();

            var scene = this.Scenes.Create<ServerMainScene>();

            this.Peer.Users.OnAdd += this.HandleUserAdd;
        }

        private void HandleUserAdd(object sender, IUser e)
        {
            this.Peer.Groups.GetById(69).Users.Add(e);
        }

        #region Event Handlers
        #endregion
    }
}
