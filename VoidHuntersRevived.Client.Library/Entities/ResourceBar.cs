using Guppy;
using Guppy.Loaders;
using Guppy.UI.Extensions;
using Guppy.UI.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Extensions.Microsoft.Xna;

namespace VoidHuntersRevived.Client.Library.Entities
{
    /// <summary>
    /// Simple entity used to render a players resource
    /// bar either above their ship or on the bottom right
    /// of the screen (for the current user)
    /// </summary>
    public class ResourceBar : Entity
    {
        #region Private Fields
        private PrimitiveBatch _primitiveBatch;
        private SpriteBatch _spriteBatch;
        private FarseerCamera2D _camera;
        private Single _hR;
        private Single _eR;
        private SpriteFont _font;
        #endregion

        #region Public Attribute
        public Ship Ship { get; internal set; }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _spriteBatch = provider.GetRequiredService<SpriteBatch>();
            _primitiveBatch = provider.GetRequiredService<PrimitiveBatch>();
            _camera = provider.GetRequiredService<FarseerCamera2D>();
            _font = provider.GetRequiredService<ContentLoader>().TryGet<SpriteFont>("font");

            this.SetLayerDepth(3);
        }

        protected override void Initialize()
        {
            base.Initialize();

            _hR = 1f;
            _eR = 1f;
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (this.Ship.Bridge != null)
            {
                // TODO: Make this all more effecient...

                var width = 50 + (Int32)(150f * _camera.Zoom);
                var hSize = width / 2;
                var pos = _camera.Project(this.Ship.Bridge.Position.ToVector3()).ToVector2().ToPoint();
                var hRect = new Rectangle(pos.X - hSize, pos.Y - 10 - (Int32)(150f * _camera.Zoom), width, 4);
                var eRect = new Rectangle(pos.X - hSize, pos.Y - 5 - (Int32)(150f * _camera.Zoom), width, 4);

                _hR = MathHelper.Lerp(_hR, (this.Ship.Bridge.Health / 100f), 0.00625f * (Single)gameTime.ElapsedGameTime.TotalMilliseconds);
                var hL = width * _hR;
                var hC = Color.Lerp(new Color(240, 0, 0), new Color(0, 240, 4, 1), _hR);

                _primitiveBatch.FillRectangle(hRect, new Color(50, 50, 50, 200));
                _primitiveBatch.DrawLine(new Vector2(hRect.Left, hRect.Top + 1), new Color(240, 0, 0), new Vector2(hRect.Left + hL, hRect.Top + 1), hC);
                _primitiveBatch.DrawLine(new Vector2(hRect.Left, hRect.Top + 2), new Color(240, 0, 0), new Vector2(hRect.Left + hL, hRect.Top + 2), hC);
                _primitiveBatch.DrawLine(new Vector2(hRect.Left, hRect.Top + 3), new Color(240, 0, 0), new Vector2(hRect.Left + hL, hRect.Top + 3), hC);
                _primitiveBatch.DrawLine(new Vector2(hRect.Left, hRect.Top + 4), new Color(240, 0, 0), new Vector2(hRect.Left + hL, hRect.Top + 4), hC);
                // _primitiveBatch.DrawRectangle(hRect, Color.White);

                _eR = MathHelper.Lerp(_eR, (this.Ship.Energy / this.Ship.MaxEnergy), 0.00625f * (Single)gameTime.ElapsedGameTime.TotalMilliseconds);
                var eL = width * _eR;
                var eC = Color.Lerp(new Color(240, 152, 0), new Color(240, 152, 0), _eR);

                _primitiveBatch.FillRectangle(eRect, new Color(50, 50, 50, 200));
                _primitiveBatch.DrawLine(new Vector2(eRect.Left, eRect.Top + 1), new Color(240, 152, 0), new Vector2(eRect.Left + eL, eRect.Top + 1), eC);
                _primitiveBatch.DrawLine(new Vector2(eRect.Left, eRect.Top + 2), new Color(240, 152, 0), new Vector2(eRect.Left + eL, eRect.Top + 2), eC);
                _primitiveBatch.DrawLine(new Vector2(eRect.Left, eRect.Top + 3), new Color(240, 152, 0), new Vector2(eRect.Left + eL, eRect.Top + 3), eC);
                _primitiveBatch.DrawLine(new Vector2(eRect.Left, eRect.Top + 4), new Color(240, 152, 0), new Vector2(eRect.Left + eL, eRect.Top + 4), eC);
                // _primitiveBatch.DrawRectangle(eRect, Color.White);

                // Draw player name
                var size = _font.MeasureString(this.Ship.Player.Name).ToPoint() + new Point(10, 0);
                var position = new Point(hRect.Left + ((hRect.Width - size.X) / 2), hRect.Top - size.Y - 1);
                var rect = new Rectangle(position, size);
                _primitiveBatch.FillRectangle(rect, new Color(0, 0, 0, 100));
                _spriteBatch.DrawString(_font, this.Ship.Player.Name, position.ToVector2() + new Vector2(5, 0), Color.White);
            }
        }
        #endregion
    }
}
