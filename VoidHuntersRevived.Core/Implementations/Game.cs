using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using VoidHuntersRevived.Core.Collections;
using VoidHuntersRevived.Core.Extensions;
using VoidHuntersRevived.Core.Factories;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Loaders;
using VoidHuntersRevived.Core.Providers;

namespace VoidHuntersRevived.Core.Implementations
{
    public class Game : ServiceLoader, IGame
    {
        private readonly ContentManager _content;
        private readonly IServiceCollection _services;
        private readonly GameWindow _window;
        public IServiceProvider Provider { get; protected set; }
        public SceneCollection Scenes { get; protected set; }

        private ServiceLoaderCollection _serviceLoaderCollection;

        public ILogger Logger { get; private set; }
        public GraphicsDeviceManager Graphics { get; set; }

        public Game(ILogger logger, GraphicsDeviceManager graphics = null, ContentManager content = null, GameWindow window = null, IServiceCollection services = null)
            : base(logger)
        {
            this.Logger = logger;
            this.Graphics = graphics;
            _services = services ?? new ServiceCollection();
            _content = content;
            _window = window;

            // Begin game initialization
            this.TryBoot();
            this.TryConfigureServices(_services);
            this.TryPreInitialize();
            this.TryInitialize();
            this.TryPostInitialize();
        }

        #region IGame Implementation
        public virtual void Draw(GameTime gameTime)
        {
            this.Scenes.Draw(gameTime);
        }

        public virtual void Update(GameTime gameTime)
        {
            this.Scenes.Update(gameTime);
        }
        #endregion

        #region Initializable Implementation
        protected override void Boot()
        {
            this.Logger.LogInformation("Booting game...");
            _serviceLoaderCollection = new ServiceLoaderCollection(
                logger: this.Logger);

            // Locks any service loaders in place, ensuring no more can be added
            _serviceLoaderCollection.Cement();

            // Call additional boot methods now
            _serviceLoaderCollection.TryBoot();
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            this.Logger.LogInformation("Configuring game services");

            // Add all IGame services now...
            services.AddSingleton<IGame>(this);
            services.AddSingleton<ILogger>(this.Logger);

            // Add all base factories
            services.AddSingleton<SceneFactory>();

            // Add all base collections
            services.AddSingleton<SceneCollection>();

            // Add all base loaders
            services.AddLoader<StringLoader>();
            services.AddLoader<ContentLoader>();
            services.AddLoader<EntityLoader>();

            if (this.Graphics != null)
            {
                services.AddSingleton<GraphicsDeviceManager>(this.Graphics);
                services.AddSingleton<GraphicsDevice>(this.Graphics.GraphicsDevice);
                services.AddSingleton<SpriteBatch>(new SpriteBatch(this.Graphics.GraphicsDevice));
            }
            if(_content != null)
            {
                services.AddSingleton<ContentManager>(_content);
            }
            if(_window != null)
            {
                services.AddSingleton<GameWindow>(_window);
            }

            _serviceLoaderCollection.TryConfigureServices(services);
        }

        protected override void PreInitialize()
        {
            this.Logger.LogInformation("PreInitializing game...");

            // After all services have been configured build a service provider
            this.Provider = _services.BuildServiceProvider();
            this.Scenes = this.Provider.GetService<SceneCollection>();

            _serviceLoaderCollection.TryPreInitialize();
        }

        protected override void Initialize()
        {
            this.Logger.LogInformation("Initializing game...");

            _serviceLoaderCollection.TryInitialize();

            // Initialize all registered loaders
            foreach (ILoader loader in this.Provider.GetServices<ILoader>())
                loader.Initialize();
        }

        protected override void PostInitialize()
        {
            this.Logger.LogInformation("PostInitializing game...");

            _serviceLoaderCollection.TryPostInitialize();
        }
        #endregion
    }
}
