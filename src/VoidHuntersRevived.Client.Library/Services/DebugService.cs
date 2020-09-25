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

namespace VoidHuntersRevived.Client.Library.Services
{
    public class DebugService : Frameable
    {
        #region Private Fields
        private SpriteBatch _spriteBatch;
        private ContentService _content;
        private SpriteFont _font;
        private String _text;
        #endregion

        #region Events
        public delegate String DebugLineDelegate(GameTime gameTime);

        public event DebugLineDelegate Lines;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _spriteBatch);
            provider.Service(out _content);

            _font = _content.Get<SpriteFont>("debug:font");
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _text = "";
            foreach (DebugLineDelegate s in this.Lines.GetInvocationList())
                _text += s(gameTime) + "\n";

            _spriteBatch.Begin();

            _spriteBatch.DrawString(_font, _text, Vector2.One * 100, Color.Red);

            _spriteBatch.End();
        }
        #endregion
    }
}
