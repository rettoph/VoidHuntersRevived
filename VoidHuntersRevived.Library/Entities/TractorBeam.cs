using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Providers;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.Interfaces;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Core.Extensions;

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// Each player instance will contain a tractor beam.
    /// The player itself should manage the tractor beam functionality
    /// </summary>
    public class TractorBeam : Entity
    {
        #region Private Fields
        private Player _player;

        private SpriteBatch _spriteBatch;

        private Texture2D _texture;
        private Vector2 _origin; // The Texture2D's origin
        #endregion

        #region Public Attributes
        /// <summary>
        /// The tractorbeams position (relative to the World)
        /// </summary>
        public Vector2 Position { get; set; }
        #endregion

        #region Constructors
        public TractorBeam(Player player, EntityInfo info, IGame game, SpriteBatch spriteBatch = null) : base(info, game)
        {
            // Save the incoming player
            _player = player;
            // Save the incoming spritebatch
            _spriteBatch = spriteBatch;
        }
        #endregion

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            // Set the default position
            this.Position = Vector2.Zero;

            if(_spriteBatch != null)
            { // Only bother loading content data if there is a spritebatch
                var contentLoader = this.Game.Provider.GetLoader<ContentLoader>();
                // Load the tractor beam texture
                _texture = contentLoader.Get<Texture2D>("texture:tractor_beam");
                // Define the tractor beam origin
                _origin = new Vector2((float)_texture.Bounds.Width / 2, (float)_texture.Bounds.Height / 2);
                // Ensure that the tractor beam is visible
                this.SetVisible(true);
            }
        }
        #endregion

        #region Frame Methods
        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Draw(
                texture: _texture,
                position: this.Position,
                sourceRectangle: _texture.Bounds,
                color: Color.White,
                rotation: 0,
                origin: _origin,
                scale: 0.01f,
                effects: SpriteEffects.None,
                layerDepth: 0);
        }
        #endregion
    }
}
