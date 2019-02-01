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
using VoidHuntersRevived.Networking.Peers;
using VoidHuntersRevived.Networking.Implementations;
using VoidHuntersRevived.Networking.Interfaces;
using Lidgren.Network;
using VoidHuntersRevived.Networking.Enums;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Client.Entities.Drivers;

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

            var entityLoader = this.Provider.GetLoader<EntityLoader>();
            entityLoader.Register<Camera>("entity:camera", "entity_name:camera", "entity_description:camera");
            entityLoader.Register<Cursor>("entity:cursor", "entity_name:cursor", "entity_description:cursor");
            entityLoader.Register<ClientFarseerEntityDriver>(handle: "entity:farseer_entity_driver");
            entityLoader.Register<ClientShipPartDriver>(handle: "entity:ship_part_driver", priority: 1);
            entityLoader.Register<ClientLocalUserPlayerDriver>(handle: "entity:player_driver:local", priority: 1);
            entityLoader.Register<ClientRemoteUserPlayerDriver>(handle: "entity:player_driver:remote", priority: 1);

            var contentLoader = this.Provider.GetLoader<ContentLoader>();
            contentLoader.Register<SpriteFont>("font:debug", "Fonts/debug");
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

#if DEBUG
            var hail = _client.CreateMessage("network:user:connection-request");
            hail.Write("tony");
            _client.Connect("localhost", 1337, hail);
#else
            Console.WriteLine("Server Ip: ");
            var server = Console.ReadLine();

            Console.WriteLine("Name: ");
            var name = Console.ReadLine();

            var hail = _client.CreateMessage("network:user:connection-request");
            hail.Write(name);
            _client.Connect(server, 1337, hail);
#endif
        }
    }
}
