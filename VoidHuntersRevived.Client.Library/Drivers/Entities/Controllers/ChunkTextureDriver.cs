using FarseerPhysics;
using Guppy;
using Guppy.Attributes;
using Guppy.Extensions.Collection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Controllers;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities.Controllers
{
    /// <summary>
    /// Driver to manage chunk textures
    /// </summary>
    [IsDriver(typeof(Chunk))]
    internal sealed class ChunkTextureDriver : Driver<Chunk>
    {
        #region Static Properties
        private static Single SnapThreshold { get; set; } = 0.25f;
        private static Single TextureScale { get => ConvertUnits.ToSimUnits(1 / ChunkTextureDriver.SnapThreshold); }

        private static BasicEffect Effect { get; set; }
        #endregion

        #region Private Fields
        private RenderTarget2D _target;
        private FarseerCamera2D _camera;
        private SpriteBatch _spriteBatch;
        private GraphicsDevice _graphics;
        private Vector2 _position;
        private BoundingBox _box;
        private Matrix _projection;
        private Boolean _dirtyTexture;
        #endregion

        #region Constructor
        public ChunkTextureDriver(GraphicsDevice graphics, SpriteBatch spriteBatch, FarseerCamera2D camera, Chunk driven) : base(driven)
        {
            _graphics = graphics;
            _spriteBatch = spriteBatch;
            _camera = camera;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            _position = new Vector2(this.driven.Position.X, this.driven.Position.Y);

            _target = new RenderTarget2D(
                graphicsDevice: _graphics,
                width: (Int32)(ConvertUnits.ToDisplayUnits(Chunk.Size) * ChunkTextureDriver.SnapThreshold),
                height: (Int32)(ConvertUnits.ToDisplayUnits(Chunk.Size) * ChunkTextureDriver.SnapThreshold));

            _projection = Matrix.CreateOrthographicOffCenter(
                    this.driven.Bounds.Left,
                    this.driven.Bounds.Right,
                    this.driven.Bounds.Bottom,
                    this.driven.Bounds.Top,
                    0f,
                    1f);

            _box = new BoundingBox(new Vector3(this.driven.Bounds.Left, this.driven.Bounds.Top, 0), new Vector3(this.driven.Bounds.Right, this.driven.Bounds.Bottom, 0));

            this.driven.OnCleaned += this.HandleChunkCleaned;
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.driven.OnCleaned -= this.HandleChunkCleaned;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(_dirtyTexture && _camera.Frustum.Contains(_box).HasFlag(ContainmentType.Intersects))
            { // If the chunk texture is dirty & in view...
                var targets = _graphics.GetRenderTargets();
                _graphics.SetRenderTarget(_target);
                _graphics.Clear(Color.Transparent);
                // _graphics.Clear(new Color(100, this.driven.Position.X / 16 % 2 == 0 ? 255 : 0, this.driven.Position.Y / 16 % 2 == 0 ? 255 : 0, 10));


                ChunkTextureDriver.Effect.Projection = _projection;

                _spriteBatch.Begin(effect: ChunkTextureDriver.Effect);
                this.driven.Components.TryDrawAll(Chunk.EmptyGameTime);
                this.driven.GetSurrounding(false).ForEach(c => c?.Components.TryDrawAll(Chunk.EmptyGameTime));
                _spriteBatch.End();

                _graphics.SetRenderTargets(targets);
                _dirtyTexture = false;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if(_camera.Frustum.Contains(_box).HasFlag(ContainmentType.Intersects))
            { // only render the chunk if its within the viewport bounds...
                if(_camera.Zoom <= ChunkTextureDriver.SnapThreshold)
                { // Draw the chunk's cached preview
                    _spriteBatch.Draw(
                        texture: _target,
                        position: _position,
                        sourceRectangle: null,
                        color: Color.White,
                        rotation: 0,
                        origin: Vector2.Zero,
                        scale: ChunkTextureDriver.TextureScale,
                        effects: SpriteEffects.None,
                        layerDepth: 0);
                }
                else
                { // Draw each component directly...
                    this.driven.Components.ForEach(e =>
                    {
                        e.TryDraw(gameTime);
                    });
                }
            }
        }
        #endregion

        #region Event Handlers
        private void HandleChunkCleaned(object sender, GameTime arg)
        {
            _dirtyTexture = true;
        }
        #endregion

        #region Static Methods
        public static void Setup(IServiceProvider provider)
        {
            // Create the chared chunk effect

            ChunkTextureDriver.Effect = provider.GetRequiredService<BasicEffect>();
            ChunkTextureDriver.Effect.TextureEnabled = true;
            ChunkTextureDriver.Effect.VertexColorEnabled = true;
            ChunkTextureDriver.Effect.View = Matrix.Identity;
        }
        #endregion
    }
}
