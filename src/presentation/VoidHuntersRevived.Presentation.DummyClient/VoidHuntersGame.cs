using Guppy.Game.Common;
using Guppy.Game.Extensions;
using Guppy.Game.MonoGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Game.Client;
using VoidHuntersRevived.Presentation.Core;

namespace VoidHuntersRevived.Application.Client
{
    public sealed class VoidHuntersGame : Microsoft.Xna.Framework.Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private VoidHuntersEngine _engine;
        private IGame? _game;


        // https://community.monogame.net/t/start-in-maximized-window/12264
        // [DllImport("SDL2.dll", CallingConvention = CallingConvention.Cdecl)]
        // public static extern void SDL_MaximizeWindow(IntPtr window);


        public VoidHuntersGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.IsMouseVisible = true;
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


            _engine = new VoidHuntersEngine("Dummy");
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
                var game = _engine.StartGame(builder =>
                {
                    builder.RegisterMonoGame(this, _graphics, this.Content, this.Window);
                });

                game.Initialize();

                game.Guppies.Create<MultiplayerGameGuppy>();

                _game = game;
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
            _game?.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _game?.Dispose();
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);

            _game?.Dispose();

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

            _game?.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GraphicsDevice.Clear(Random.Shared.Next(0, 2) == 0 ? Color.Black : Color.Gray);

            _game?.Draw(gameTime);
        }
    }
}
