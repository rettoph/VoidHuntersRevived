using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guppy.MonoGame.Strategies.PublishStrategies;
using Guppy;
using Guppy.Common;
using Guppy.MonoGame.Services;
using Microsoft.Extensions.DependencyInjection;
using Guppy.Providers;
using VoidHuntersRevived.Domain.Server;

namespace VoidHuntersRevived.Domain.Client
{
    public sealed class VoidHuntersGame : Microsoft.Xna.Framework.Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private IServiceProvider? _provider;
        private IScoped<ClientGameGuppy>? _client;
        private IScoped<ServerGameGuppy>? _server;
        private IGlobal<IGameComponentService>? _globals;


        // https://community.monogame.net/t/start-in-maximized-window/12264
        // [DllImport("SDL2.dll", CallingConvention = CallingConvention.Cdecl)]
        // public static extern void SDL_MaximizeWindow(IntPtr window);


        public VoidHuntersGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.IsMouseVisible = false;
            this.Window.AllowUserResizing = true;
            this.IsFixedTimeStep = false;

            _graphics.PreparingDeviceSettings += (s, e) =>
            {
                e.GraphicsDeviceInformation.PresentationParameters.PresentationInterval = PresentInterval.Immediate;
                e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            };
            _graphics.SynchronizeWithVerticalRetrace = false;
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            _graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            _provider = new GuppyEngine(new[] { typeof(GameGuppy).Assembly, typeof(ClientGameGuppy).Assembly })
                        .ConfigureMonoGame<LastGuppyPublishStrategy>(this, _graphics, this.Content, this.Window)
                        .ConfigureECS()
                        .ConfigureNetwork()
                        .ConfigureResources()
                        .ConfigureUI()
                        .ConfigureNetworkUI()
                        .Build();

            return;
            // SDL_MaximizeWindow(this.Window.Handle);
            Task.Run(() =>
            {
                try
                {
                    _provider = new GuppyEngine(new[] { typeof(GameGuppy).Assembly, typeof(ClientGameGuppy).Assembly })
                        .ConfigureMonoGame<LastGuppyPublishStrategy>(this, _graphics, this.Content, this.Window)
                        .ConfigureECS()
                        .ConfigureNetwork()
                        .ConfigureResources()
                        .ConfigureUI()
                        .ConfigureNetworkUI()
                        .Build();
                }
                catch (Exception ex)
                {

                }
            });
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);

            Environment.Exit(0);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            base.Update(gameTime);

            // _server ??= _provider?.GetRequiredService<IGuppyProvider>().Create<ServerGameGuppy>();
            _client ??= _provider?.GetRequiredService<IGuppyProvider>().Create<ClientGameGuppy>();

            _globals ??= _provider?.GetRequiredService<IGlobal<IGameComponentService>>();

            _globals?.Instance.Update(gameTime);

            _server?.Instance.Update(gameTime);
            _client?.Instance.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.GraphicsDevice.Clear(Color.Black);

            _globals?.Instance.Draw(gameTime);

            _server?.Instance.Draw(gameTime);
            _client?.Instance.Draw(gameTime);
        }
    }
}
