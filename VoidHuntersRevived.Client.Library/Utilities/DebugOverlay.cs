using Guppy;
using Guppy.Loaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoidHuntersRevived.Client.Library.Utilities
{
    public class DebugOverlay : Frameable
    {
        #region Private Fields
        private SpriteFont _font;
        private SpriteBatch _spriteBatch;
        private List<Func<GameTime, String>> _lines;
        #endregion

        #region Constructors
        public DebugOverlay(ContentLoader content, SpriteBatch spriteBatch)
        {
            _font = content.TryGet<SpriteFont>("font");
            _spriteBatch = spriteBatch;
            _lines = new List<Func<GameTime, String>>();
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

#if DEBUG
            if(_lines.Any())
            {
                _spriteBatch.Begin();
                _spriteBatch.DrawString(
                    spriteFont: _font,
                    text: _lines.Select(l => l(gameTime)).Aggregate((l1, l2) => l1 + "\n" + l2),
                    position: new Vector2(25, 25),
                    color: Color.White);
                _spriteBatch.End();
            }
#endif
        }
        #endregion

        #region Helper Methods
        public void AddLine(Func<GameTime, String> line)
        {
            _lines.Add(line);
        }
        #endregion

    }
}
