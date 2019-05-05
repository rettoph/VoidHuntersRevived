﻿using Guppy;
using Guppy.Loggers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Client
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private GuppyLoader _guppy;
        private VoidHuntersClientGame _game;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _guppy = new GuppyLoader(new ConsoleLogger());

            this.Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.IsMouseVisible = true;
            this.Window.AllowUserResizing = true;
            this.Window.Title = "Void Hunters Revived 0.0.2";
            // this.Window.IsBorderless = true;

            _graphics.HardwareModeSwitch = false;
            // _graphics.ToggleFullScreen();

            _guppy.ConfigureMonogame(_graphics, this.Window, this.Content);
            _guppy.Initialize();

            _game = _guppy.Games.Create<VoidHuntersClientGame>();
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Update(gameTime);

            _game.Draw(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _game.Update(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);

            Environment.Exit(0);
        }
    }
}
