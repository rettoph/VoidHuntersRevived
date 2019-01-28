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
using VoidHuntersRevived.Client.Entities;
using VoidHuntersRevived.Client.Scenes;
using VoidHuntersRevived.Core.Factories;
using VoidHuntersRevived.Client.Configurations;
using VoidHuntersRevived.Client.Entities.Ships;
using VoidHuntersRevived.Networking.Peers;
using VoidHuntersRevived.Networking.Implementations;
using VoidHuntersRevived.Networking.Interfaces;
using Lidgren.Network;
using VoidHuntersRevived.Networking.Enums;

namespace VoidHuntersRevived.Client
{
    class VoidHuntersRevivedClientGame : VoidHuntersRevivedGame
    {
        private ClientPeer _client;

        public VoidHuntersRevivedClientGame(ILogger logger, GraphicsDeviceManager graphics = null, ContentManager content = null, GameWindow window = null, IServiceCollection services = null) : base(logger, graphics, content, window, services)
        {
            
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.AddSingleton<IPeer>(new ClientPeer("vhr", this, this.Logger));
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            var sceneFactory = this.Provider.GetService<SceneFactory>();
            sceneFactory.ApplyConfiguration<ClientMainScene>(new MainSceneClientConfiguration());

            var stringLoader = this.Provider.GetLoader<StringLoader>();
            stringLoader.Register("entity_name:camera", "Camera");
            stringLoader.Register("entity_description:camera", "A basic camera that auto adjusts for Farseer positioning via BasicEffect.");

            stringLoader.Register("entity_name:cursor", "Cursor");
            stringLoader.Register("entity_description:cursor", "The current client's cursor.");

            stringLoader.Register("entity_name:ship:user:current", "Current User Ship");
            stringLoader.Register("entity_description:ship:user:current", "A ship controllable by the current user.");

            var entityLoader = this.Provider.GetLoader<EntityLoader>();
            entityLoader.Register<Camera>("entity:camera", "entity_name:camera", "entity_description:camera");
            entityLoader.Register<Cursor>("entity:cursor", "entity_name:cursor", "entity_description:cursor");
            entityLoader.Register<CurrentUserShip>("entity:ship:user:current", "entity_name:ship:user:current", "entity_description:ship:user:current");

            var contentLoader = this.Provider.GetLoader<ContentLoader>();
            contentLoader.Register<Texture2D>("texture:connection_node:male", "Sprites/male-connection");
            contentLoader.Register<Texture2D>("texture:connection_node:female", "Sprites/female-connection");
        }

        protected override void Initialize()
        {
            base.Initialize();

            _client = this.Provider.GetService<IPeer>() as ClientPeer;

            var scene = this.Scenes.Create<ClientMainScene>();
        }

        protected override void PostInitialize()
        {
            base.PostInitialize();

            var hail = _client.CreateMessage("network:user:connection-request");
            hail.Write("Rettoph");
            _client.Connect("localhost", 1337, hail);
        }
    }
}
