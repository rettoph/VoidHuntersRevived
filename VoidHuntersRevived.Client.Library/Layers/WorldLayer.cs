using VoidHuntersRevived.Client.Library.Entities;
using VoidHuntersRevived.Client.Library.Scenes;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Entities.Ammunitions;
using Guppy;
using Guppy.Extensions.Collection;
using Guppy.Network.Peers;
using Guppy.Pooling.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Guppy.Loaders;

namespace VoidHuntersRevived.Client.Library.Layers
{
    public class WorldLayer : BloomLayer
    {
        #region Private Fields
        private FarseerCamera2D _camera;
        private BasicEffect _effect;
        private SpriteBatch _spriteBatch;
        private GraphicsDevice _graphics;
        private DebugOverlay _debug;
        private IPool<Projectile> _projectiles;
        private ContentLoader _content;
        #endregion

        #region Constructor
        public WorldLayer(ContentLoader content, SpriteBatch spriteBatch, GraphicsDevice graphics, GameWindow window, IPool<Projectile> projectiles, DebugOverlay debug, ClientWorldScene scene) : base(content, spriteBatch, graphics, window)
        {
            _projectiles = projectiles;
            _debug = debug;
            _graphics = graphics;
            _spriteBatch = spriteBatch;
            _camera = scene.Camera;
            _content = content;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            _debug.AddLine(() => $"Entities: {this.entities.Count()}");
            _debug.AddLine(() => $"Pooled Projectiles: {_projectiles.Count()}");

            _effect = new BasicEffect(_graphics)
            {
                TextureEnabled = true,
                VertexColorEnabled = true
            };

            this.SetDrawOrder(100);
        }
        #endregion

        #region Frame Methods 
        protected override void Draw(GameTime gameTime)
        {
            this.bloom.BloomThreshold = 0.1f;
            this.bloom.BloomStrengthMultiplier = 0.75f;
            this.bloom.BloomUseLuminance = false;
            this.bloom.BloomPreset = Effects.BloomFilter.BloomPresets.SuperWide;

            base.Draw(gameTime);

            // Update the internal effect
            _effect.Projection = _camera.Projection;
            _effect.View = _camera.View;

            // Draw all entities
            _spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp, effect: _effect);
            this.entities.TryDraw(gameTime);
            _spriteBatch.End();
        }
        #endregion
    }
}
