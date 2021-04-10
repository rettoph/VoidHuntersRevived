using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoidHuntersRevived.Windows.Library.Services
{
    public class DebugService : Frameable
    {
        #region Private Fields
        private SpriteBatch _spriteBatch;
        private ContentService _content;
        private SpriteFont _font;
        private String _text;
        private Vector2 _position;
        private Texture2D _background;
        private GraphicsDevice _graphics;
        #endregion

        #region Events
        public delegate String DebugLineDelegate(GameTime gameTime);

        public event DebugLineDelegate Lines;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _position = Vector2.One * 15;

            provider.Service(out _spriteBatch);
            provider.Service(out _content);
            provider.Service(out _graphics);

            _background = new Texture2D(_graphics, 1, 1);
            _background.SetData<Color>(new Color[] { Color.White });

            _font = _content.Get<SpriteFont>("debug:font");
        }

        protected override void Release()
        {
            base.Release();

            _spriteBatch = null;
            _content = null;
            _graphics = null;
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _text = "";
            foreach (DebugLineDelegate s in this.Lines.GetInvocationList())
                _text += s(gameTime) + "\n";

            _text = _text.Trim('\n');

            var size = _font.MeasureString(_text) + _position + _position;

            _spriteBatch.Begin();
            _spriteBatch.Draw(_background, new Rectangle(0, 0, (Int32)size.X, (Int32)size.Y), new Color(0, 0, 0, 50));
            _spriteBatch.DrawString(_font, _text, _position, Color.White);
            _spriteBatch.End();
        }
        #endregion
    }
}
