using FarseerPhysics;
using Guppy;
using Guppy.Attributes;
using Guppy.Extensions.Collection;
using Guppy.Utilities.Cameras;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Client.Library.Scenes;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Client.Library.Drivers
{
    [IsDriver(typeof(Chunk))]
    internal sealed class ClientChunkDriver : Driver<Chunk>
    {
        public static Single SnapThreshold = 0.5f;

        #region Private Fields
        private GraphicsDevice _graphics;
        private SpriteBatch _spriteBatch;
        private Vector2 _position;
        private BasicEffect _effect;
        private ClientWorldScene _scene;
        private Vector3 _p;
        private Rectangle _bounds;
        private BoundingBox _box;
        private Boolean _render;
        private Boolean _dirty;
        private Dictionary<Single, RenderTarget2D> _textures;
        private Dictionary<Single, Boolean> _dirtyTextures;
        #endregion

        #region Constructor
        public ClientChunkDriver(ClientWorldScene scene, GraphicsDevice graphics, SpriteBatch spriteBatch, Chunk driven) : base(driven)
        {
            _graphics = graphics;
            _spriteBatch = spriteBatch;
            _scene = scene;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            // Create position coordinates for internal use
            _position = new Vector2(this.driven.Position.X, this.driven.Position.Y);
            _bounds = new Rectangle(_position.ToPoint(), new Vector2(Chunk.Size, Chunk.Size).ToPoint());
            _box = new BoundingBox(new Vector3(_bounds.Left, _bounds.Top, 0), new Vector3(_bounds.Right, _bounds.Bottom, 0));
            _textures = new Dictionary<Single, RenderTarget2D>();
            _dirtyTextures = new Dictionary<Single, Boolean>();

            _p = new Vector3(_position, 0);

            _effect = new BasicEffect(_graphics)
            {
                TextureEnabled = true,
                VertexColorEnabled = true,
                View = Matrix.Identity,
                Projection = Matrix.CreateOrthographicOffCenter(_position.X, _position.X + Chunk.Size, _position.Y + Chunk.Size, _position.Y, 0, 1)
            };

            this.driven.Events.TryAdd<GameTime>("cleaned", this.HandleChunkCleaned);
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (this.IsDirty() && _scene.Camera.Frustum.Contains(_box) != ContainmentType.Disjoint)
            {
                if((_render = this.CanRender()))
                    this.CleanTexture(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if(_scene.ChunkScale <= ClientChunkDriver.SnapThreshold)
            { // Draw the cached texture when zoomed out
                if (_render && !_dirtyTextures[_scene.ChunkScale] && _scene.Camera.Frustum.Contains(_box) != ContainmentType.Disjoint)
                    _spriteBatch.Draw(
                        _textures[_scene.ChunkScale],
                        _bounds,
                        Color.White);
            }
            else
            { // Just draw internal components directly when zoomed in
                this.driven.Components.TryDrawAll(gameTime);
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Redraw the current texture
        /// </summary>
        /// <param name="gameTime"></param>
        private void CleanTexture(GameTime gameTime)
        {
            this.TryCreateTexture();

            _graphics.SetRenderTarget(_textures[_scene.ChunkScale]);
            // _graphics.Clear(new Color(_position.X / Chunk.Size % 2 == 0 ? 100 : 255, 0, _position.Y / Chunk.Size % 2 == 0 ? 100 : 255, 100));
            _graphics.Clear(Color.Transparent);

            _spriteBatch.Begin(effect: _effect);


            this.driven.Components.TryUpdateAll(gameTime);
            this.driven.Components.TryDrawAll(gameTime);

            this.driven.GetOrCreateSurrounding().ForEach(c =>
            {
                c.Components.TryUpdateAll(gameTime);
                c.Components.TryDrawAll(gameTime);
            });
            _spriteBatch.End();


            // Reset graphics device
            _graphics.SetRenderTarget(null);

            // Mark this texture as clean
            _dirtyTextures[_scene.ChunkScale] = false;
        }

        private Boolean CanRender()
        {
            if (this.driven.Components.Any())
                return true;

            foreach (Chunk chunk in this.driven.GetSurrounding())
                if (chunk.Components.Any())
                    return true;

            return false;
        }

        /// <summary>
        /// Validate that a texture exists for the chunk at
        /// the current target zool level.
        /// 
        /// Returns true if a texture was created
        /// </summary>
        /// <returns></returns>
        private Boolean TryCreateTexture()
        {
            if(!_textures.ContainsKey(_scene.ChunkScale))
            { // If the scale has changed, create a new texture
                // Create a new texture
                _textures[_scene.ChunkScale] = new RenderTarget2D(
                    graphicsDevice: _graphics, 
                    width: (Int32)Math.Ceiling(Chunk.Size * ConvertUnits.DisplayUnitsToSimUnitsRatio * _scene.ChunkScale), 
                    height: (Int32)Math.Ceiling(Chunk.Size * ConvertUnits.DisplayUnitsToSimUnitsRatio * _scene.ChunkScale));

                return true;
            }

            return false;
        }

        private Boolean IsDirty()
        {
            // No need to render textures at this scale level
            if (_scene.ChunkScale > ClientChunkDriver.SnapThreshold)
                return false;

            if (_dirtyTextures.TryGetValue(_scene.ChunkScale, out _dirty))
                return _dirty;

            _dirtyTextures[_scene.ChunkScale] = true;
            return _dirtyTextures[_scene.ChunkScale];
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Clean the current chunk's texture.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private void HandleChunkCleaned(object sender, GameTime arg)
        {
            // Mark all internal textures as dirty
            _dirtyTextures.Keys.ToList().ForEach(k => _dirtyTextures[k] = true);
            _render = false;
        }
        #endregion
    }
}
