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

namespace VoidHuntersRevived.Client
{
    class VoidHuntersRevivedClientGame : VoidHuntersRevivedGame
    {
        public ClientPeer Client { get; private set; }

        public VoidHuntersRevivedClientGame(ILogger logger, GraphicsDeviceManager graphics = null, ContentManager content = null, GameWindow window = null, IServiceCollection services = null) : base(logger, graphics, content, window, services)
        {
            
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            var sceneFactory = this.Provider.GetService<SceneFactory>();
            sceneFactory.ApplyConfiguration<MainSceneClient>(new MainSceneClientConfiguration());

            var stringLoader = this.Provider.GetLoader<StringLoader>();
            stringLoader.Register("entity_name:camera", "Camera");
            stringLoader.Register("entity_description:camera", "A basic camera that auto adjusts for Farseer positioning via BasicEffect.");

            stringLoader.Register("entity_name:cursor", "Cursor");
            stringLoader.Register("entity_description:cursor", "The current client's cursor.");

            stringLoader.Register("entity_name:ship:current_client", "Current Client Ship");
            stringLoader.Register("entity_description:ship:current_client", "A ship controllable by the current client.");

            var entityLoader = this.Provider.GetLoader<EntityLoader>();
            entityLoader.Register<Camera>("entity:camera", "entity_name:camera", "entity_description:camera");
            entityLoader.Register<Cursor>("entity:cursor", "entity_name:cursor", "entity_description:cursor");
            entityLoader.Register<CurrentClientShip>("entity:ship:current_client", "entity_name:ship:current_client", "entity_description:ship:current_client");

            var contentLoader = this.Provider.GetLoader<ContentLoader>();
            contentLoader.Register<Texture2D>("texture:connection_node:male", "Sprites/male-connection");
            contentLoader.Register<Texture2D>("texture:connection_node:female", "Sprites/female-connection");
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Create the peer
            this.Client = new ClientPeer("vhr", this, this.Logger);
            this.Peer = this.Client;
        }

        protected override void PostInitialize()
        {
            base.PostInitialize();

            this.Client.Connect("localhost", 1337);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
