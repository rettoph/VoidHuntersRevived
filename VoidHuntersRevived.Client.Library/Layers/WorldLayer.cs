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

namespace VoidHuntersRevived.Client.Library.Layers
{
    public class WorldLayer : Layer
    {
        #region Private Fields
        private FarseerCamera2D _camera;
        private BasicEffect _effect;
        private SpriteBatch _spriteBatch;
        private GraphicsDevice _graphics;
        private DebugOverlay _debug;
        private IPool<Projectile> _projectiles;
        #endregion

        #region Constructor
        public WorldLayer(IPool<Projectile> projectiles, DebugOverlay debug, GraphicsDevice graphics, SpriteBatch spriteBatch, ClientWorldScene scene)
        {
            _projectiles = projectiles;
            _debug = debug;
            _graphics = graphics;
            _spriteBatch = spriteBatch;
            _camera = scene.Camera;
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
