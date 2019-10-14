using Guppy;
using Guppy.Loaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoidHuntersRevived.Client.Library.Entities
{
    public class DebugOverlay : Entity
    {
        #region Private Fields
        private List<Func<String>> _lines;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        #endregion

        #region Constructor
        public DebugOverlay(SpriteBatch spriteBatch, ContentLoader content)
        {
            _font = content.TryGet<SpriteFont>("font");
            _spriteBatch = spriteBatch;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _lines = new List<Func<String>>();

            this.SetLayerDepth(1);
        }

        public override void Dispose()
        {
            base.Dispose();

            _lines.Clear();
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (_lines.Any())
            {
                var t = _lines.Select(f => f()).Aggregate((s1, s2) => s1 += "\n" + s2);
                _spriteBatch.Begin();
                _spriteBatch.DrawString(_font, t, Vector2.One * 15, Color.White);
                _spriteBatch.End();
            }
        }
        #endregion

        #region Helper Methods
        public void AddLine(Func<String> line)
        {
            _lines.Add(line);
        }
        #endregion
    }
}
