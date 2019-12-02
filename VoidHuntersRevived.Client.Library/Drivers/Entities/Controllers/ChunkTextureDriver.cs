using FarseerPhysics;
using Guppy;
using Guppy.Attributes;
using Guppy.Extensions.Collection;
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
        #endregion

        #region Private Fields
        private RenderTarget2D _target;
        private FarseerCamera2D _camera;
        private BasicEffect _effect;
        private SpriteBatch _spriteBatch;
        private GraphicsDevice _graphics;
        private Vector2 _position;
        private BoundingBox _box;
        #endregion

        #region Constructor
        public ChunkTextureDriver(GraphicsDevice graphics, SpriteBatch spriteBatch, BasicEffect effect, FarseerCamera2D camera, Chunk driven) : base(driven)
        {
            _graphics = graphics;
            _spriteBatch = spriteBatch;
            _effect = effect;
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

            _effect.TextureEnabled = true;
            _effect.VertexColorEnabled = true;
            _effect.View = Matrix.Identity;
            _effect.Projection = Matrix.CreateOrthographicOffCenter(
                    this.driven.Bounds.Left,
                    this.driven.Bounds.Right,
                    this.driven.Bounds.Bottom,
                    this.driven.Bounds.Top,
                    0f,
                    1f);

            _box = new BoundingBox(new Vector3(this.driven.Bounds.Left, this.driven.Bounds.Top, 0), new Vector3(this.driven.Bounds.Right, this.driven.Bounds.Bottom, 0));

            this.driven.Events.TryAdd<GameTime>("cleaned", this.HandleChunkCleaned);
        }
        #endregion

        #region Frame Methods
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
            var targets = _graphics.GetRenderTargets();
            _graphics.SetRenderTarget(_target);
            _graphics.Clear(Color.Transparent);
            // _graphics.Clear(new Color(100, this.driven.Position.X / 16 % 2 == 0 ? 255 : 0, this.driven.Position.Y / 16 % 2 == 0 ? 255 : 0, 10));

            _spriteBatch.Begin(effect: _effect);
            this.driven.Components.TryDrawAll(Chunk.EmptyGameTime);
            this.driven.GetSurrounding().ForEach(c => c.Components.TryDrawAll(Chunk.EmptyGameTime));
            _spriteBatch.End();

            _graphics.SetRenderTargets(targets);
        }
        #endregion
    }
}
