using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Extensions.Utilities;
using Guppy.Utilities;
using Guppy.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Library.Entities.Players;

namespace VoidHuntersRevived.Client.Library.Entities
{
    /// <summary>
    /// The primary nameplate rendered over
    /// a distinct player.
    /// </summary>
    public class PlayerNameplate : Entity
    {
        #region Private Fields
        private PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private SpriteBatch _spriteBatch;
        private Camera2D _worldCamera;
        private SpriteFont _font;
        private Texture2D _foreground;
        private Texture2D _background;

        private Vector2 _textureOffset;
        private Vector2 _nameOffset;

        private Single _healthPercent;
        private Single _energyPercent;
        private Vector2 _position;
        private Vector2 _ship;
        #endregion

        #region Internal Properties
        protected internal Player player { protected get; set; }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _primitiveBatch);
            provider.Service(out _spriteBatch);
            provider.Service(out _worldCamera);

            _font = provider.GetContent<SpriteFont>("font:player:nameplate:1");
            _foreground = provider.GetContent<Texture2D>("sprite:player-nameplate:foreground");
            _background = provider.GetContent<Texture2D>("sprite:player-nameplate:background");

            this.LayerGroup = VHR.LayersContexts.HeadsUpDisplay.Group.GetValue();
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            _textureOffset = new Vector2()
            {
                X = _foreground.Bounds.Width / 2,
                Y = _foreground.Bounds.Height / 2
            };

            _nameOffset = new Vector2()
            {
                X = _font.MeasureString(this.player.Name).X / 2,
                Y = _foreground.Bounds.Height / 2 + _font.LineSpacing + 2
            };
        }
        #endregion

        #region Lifecycle Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (this.player.Ship?.Bridge == default)
                return;

            var healthPercent = this.player.Ship.Bridge.Health / this.player.Ship.Bridge.Context.MaxHealth;
            _healthPercent = MathHelper.Lerp(_healthPercent, healthPercent, (Single)gameTime.ElapsedGameTime.TotalSeconds * 10f);

            var energyPercent = this.player.Ship.ManaPercentage;
            _energyPercent = MathHelper.Lerp(_energyPercent, energyPercent, (Single)gameTime.ElapsedGameTime.TotalSeconds * 10f);

            _ship = _worldCamera.Project(this.player.Ship.Bridge.Position);
            var position = _ship - new Vector2(0, 25) - (new Vector2(0, 3) * _worldCamera.Zoom);

            if (Vector2.Distance(_position, position) > 500)
                _position = position;
            else
                _position = Vector2.Lerp(_position, position, (Single)gameTime.ElapsedGameTime.TotalSeconds * 10f);
        }


        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (this.player.Ship?.Bridge == default)
                return;

            _spriteBatch.Draw(_background, _position - _textureOffset, this.player.Ship.Color);

            _primitiveBatch.DrawLine(new Color(Color.Gray, 10), _ship, _position);
            _primitiveBatch.TryFlushLineVertices(true);

            var healthbar = new Rectangle((_position - _textureOffset + new Vector2(5, 4)).ToPoint(), new Point((Int32)(104 * _healthPercent), 7));
            _primitiveBatch.DrawRectangle(Color.Lerp(this.player.Ship.Color, Color.Lerp(Color.Red, Color.Green, this.player.Ship.Bridge.Health / this.player.Ship.Bridge.Context.MaxHealth), 0.75f), healthbar);
            var energyBar = new Rectangle((_position - _textureOffset + new Vector2(5, 13)).ToPoint(), new Point((Int32)(104 * _energyPercent), 3));
            _primitiveBatch.DrawRectangle(Color.Yellow, energyBar);

            // We want to add a marker ever 25 energy within the energy bar. This will calculate those lines.
            var segments = this.player.Ship.MaxMana / 25;
            var size = 104 / segments;
            for (var i=1; i< segments; i++)
            {
                var x = 4 + (i * size);

                _primitiveBatch.DrawLine(
                    c1: Color.Gray,
                    p1: _position - _textureOffset + new Vector2(x, 13),
                    c2: Color.Gray,
                    p2: _position - _textureOffset + new Vector2(x, 17));
            }

            _primitiveBatch.TryFlushTriangleVertices(true);
            _primitiveBatch.TryFlushLineVertices(true);

            _spriteBatch.DrawString(_font, this.player.Name, _position - _nameOffset, this.player.Ship.Color);
            _spriteBatch.Draw(_foreground, _position - _textureOffset, this.player.Ship.Color);
        }
        #endregion
    }
}
