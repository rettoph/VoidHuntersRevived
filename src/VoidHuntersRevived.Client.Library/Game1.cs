using Guppy;
using Guppy.Extensions;
using Guppy.Network.Extensions;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace VoidHuntersRevived.Client.Library
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GuppyLoader guppy;
        ClientVoidHuntersRevivedGame game;

        // https://community.monogame.net/t/start-in-maximized-window/12264
        [DllImport("SDL2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_MaximizeWindow(IntPtr window);

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            guppy = new GuppyLoader();

            this.IsMouseVisible = true;
            this.Window.AllowUserResizing = true;
            this.IsFixedTimeStep = false;

            this.graphics.PreparingDeviceSettings += (s, e) =>
            {
                e.GraphicsDeviceInformation.PresentationParameters.PresentationInterval = PresentInterval.Immediate;
                e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            };
            this.graphics.SynchronizeWithVerticalRetrace = false;
            this.graphics.GraphicsProfile = GraphicsProfile.HiDef;
            this.graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            SDL_MaximizeWindow(Window.Handle);


            this.game = guppy.ConfigureMonoGame(graphics, this.Content, this.Window)
                .ConfigureInput()
                .ConfigureTerminal("DiagnosticsFont")
                .Initialize()
                .BuildGame<ClientVoidHuntersRevivedGame>();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

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
            this.game.TryRelease();
            this.game = null;
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

            game.TryUpdate(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            game.TryDraw(gameTime);

            base.Draw(gameTime);
        }
    }
}
