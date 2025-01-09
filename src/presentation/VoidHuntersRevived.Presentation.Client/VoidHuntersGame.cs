using Autofac;
using Guppy.Core.Network.Common.Enums;
using Guppy.Core.Network.Extensions;
using Guppy.Game;
using Guppy.Game.Common.Extensions;
using Guppy.Game.MonoGame.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Domain.Common.Constants;
using VoidHuntersRevived.Domain.Extensions;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Game.Client;
using VoidHuntersRevived.Game.Client.Extensions;
using VoidHuntersRevived.Game.Core.Extensions;
using VoidHuntersRevived.Game.Server;
using VoidHuntersRevived.Game.Server.Extensions;
using VoidHuntersRevived.Presentation.Core;
using VoidHuntersRevived.Presentation.Core.Extensions;

namespace VoidHuntersRevived.Presentation.Client
{
    public sealed class VoidHuntersGame : Microsoft.Xna.Framework.Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private GameEngine? _engine;
        private readonly bool _internalServer;


        // https://community.monogame.net/t/start-in-maximized-window/12264
        // [DllImport("SDL2.dll", CallingConvention = CallingConvention.Cdecl)]
        // public static extern void SDL_MaximizeWindow(IntPtr window);


        public VoidHuntersGame(bool internalServer = false)
        {
            this._internalServer = internalServer;
            this._graphics = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";

            this.IsMouseVisible = true;
            this.Window.AllowUserResizing = true;
            this.IsFixedTimeStep = false;

            this._graphics.PreparingDeviceSettings += (s, e) =>
            {
                this._graphics.PreferMultiSampling = true;
                e.GraphicsDeviceInformation.PresentationParameters.MultiSampleCount = 8;
                e.GraphicsDeviceInformation.PresentationParameters.PresentationInterval = PresentInterval.Immediate;
                e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            };
            this._graphics.SynchronizeWithVerticalRetrace = false;
            this._graphics.GraphicsProfile = GraphicsProfile.HiDef;
            this._graphics.ApplyChanges();
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

            // SDL_MaximizeWindow(this.Window.Handle);
            Task.Run(() =>
            {
                var engine = new GameEngine(VoidHuntersContextBuilder.ClientContext, builder =>
                {
                    builder.RegisterMonoGameServices(this, this._graphics, this.Content, this.Window)
                        .RegisterCoreNetworkServices()
                        .RegisterDomainServices()
                        .RegisterGameCoreServices()
                        .RegisterGameServerServices()
                        .RegisterGameClientServices()
                        .RegisterPresentationCoreServices();

                    builder.RegisterType<ClientSerilogSinkConfigurator>().As<ISerilogSinkConfigurator>().InstancePerLifetimeScope();
                }).Start();

                if (this._internalServer)
                {
                    engine.Scenes.Create<ServerGameScene>(configuration =>
                    {
                        configuration.WithContainerBuilder(builder =>
                        {
                            builder.RegisterNetScope<IStrategy>(PeerType.Server, NetScopeIds.Game);
                        });
                    });
                }

                engine.Scenes.Create<MultiplayerGameScene>(configuration =>
                {
                    configuration.WithContainerBuilder(builder =>
                    {
                        builder.RegisterNetScope<IStrategy>(PeerType.Client, NetScopeIds.Game);
                    });
                });
                //_engine.Guppies.Create<EditorGuppy>();

                this._engine = engine;
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
            this._engine?.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            this._engine?.Dispose();
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);

            this._engine?.Dispose();

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

            this._engine?.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (this._engine is null)
            {
                return;
            }

            this.GraphicsDevice.Clear(Color.Black);

            this._engine?.Draw(gameTime);
        }
    }
}