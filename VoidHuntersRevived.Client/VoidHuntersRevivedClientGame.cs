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

namespace VoidHuntersRevived.Client
{
    class VoidHuntersRevivedClientGame : VoidHuntersRevivedGame
    {
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

            var entityLoader = this.Provider.GetLoader<EntityLoader>();
            entityLoader.Register<Camera>("entity:camera", "entity_name:camera", "entity_description:camera");
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Create a new client scene instance
            this.Scenes.Create<MainSceneClient>();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
