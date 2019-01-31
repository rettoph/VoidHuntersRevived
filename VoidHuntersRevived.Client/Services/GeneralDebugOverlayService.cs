using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Client.Scenes;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Providers;
using VoidHuntersRevived.Core.Extensions;
using VoidHuntersRevived.Library.Entities.Players;

namespace VoidHuntersRevived.Client.Services
{
    class GeneralDebugOverlayService : SceneObject, ISceneService
    {
        private SpriteBatch _spriteBatch;
        private ClientMainScene _scene;
        private GraphicsDevice _graphics;
        private ContentLoader _contentLoader;

        private SpriteFont _font;
        private Vector2[] _lines;

        public GeneralDebugOverlayService(IServiceProvider provider, GraphicsDevice graphics, SpriteBatch spriteBatch, IGame game) : base(game)
        {
            _graphics = graphics;
            _contentLoader = provider.GetLoader<ContentLoader>();
            _spriteBatch = spriteBatch;

            this.Enabled = true;
            this.Visible = true;

            _lines = new Vector2[] {
                new Vector2(10, 10),
                new Vector2(10, 30),
                new Vector2(10, 50),
                new Vector2(10, 70)
            };
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            _spriteBatch.DrawString(_font, "Incoming Message Count: " + _scene.MessageCount, _lines[0], Color.White);
            _spriteBatch.DrawString(_font, $"Entities: {_scene.Entities.Count()}, Enabled: {_scene.Entities.EnabledCount}, Visible: {_scene.Entities.VisibleCount}", _lines[1], Color.White);
            _spriteBatch.DrawString(_font, "Available Female Nodes: " + _scene.CurrentPlayer?.AvailableFemaleConnectionNodes?.Length, _lines[2], Color.White);
            _spriteBatch.DrawString(_font, "Users: " + _scene.Group.Users.Count(), _lines[3], Color.White);

            _spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        protected override void Boot()
        {
            // throw new NotImplementedException();
        }

        protected override void Initialize()
        {
            _scene = this.Scene as ClientMainScene;
            _font = _contentLoader.Get<SpriteFont>("font:debug");
        }

        protected override void PostInitialize()
        {
            // throw new NotImplementedException();
        }

        protected override void PreInitialize()
        {
            // throw new NotImplementedException();
        }
    }
}
