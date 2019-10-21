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
using System.Text;
using VoidHuntersRevived.Client.Library.Scenes;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Client.Library.Drivers
{
    [IsDriver(typeof(Chunk))]
    internal sealed class ClientChunkDriver : Driver<Chunk>
    {
        #region Private Fields
        private RenderTarget2D _texture;
        private GraphicsDevice _graphics;
        private SpriteBatch _spriteBatch;
        private Vector2 _position;
        private BasicEffect _effect;
        private Camera2D _camera;
        private Vector3 _p;
        #endregion

        #region Constructor
        public ClientChunkDriver(ClientWorldScene scene, GraphicsDevice graphics, SpriteBatch spriteBatch, Chunk driven) : base(driven)
        {
            _graphics = graphics;
            _spriteBatch = spriteBatch;
            _camera = scene.Camera;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            // Create a new empty texture for the chunk
            _texture = new RenderTarget2D(_graphics, (Int32)Math.Ceiling(Chunk.Size * ConvertUnits.DisplayUnitsToSimUnitsRatio), (Int32)Math.Ceiling(Chunk.Size * ConvertUnits.DisplayUnitsToSimUnitsRatio));

            _position = new Vector2(this.driven.Position.X, this.driven.Position.Y);
            _p = new Vector3(_position, 0);

            _effect = new BasicEffect(_graphics)
            {
                TextureEnabled = true,
                VertexColorEnabled = true
            };

            this.driven.Events.TryAdd<GameTime>("cleaned", this.Clean);
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if(_camera.Frustum.Contains(_p) == ContainmentType.Contains)
                _spriteBatch.Draw(
                    _texture,
                    _position,
                    _texture.Bounds,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    ConvertUnits.SimUnitsToDisplayUnitsRatio,
                    SpriteEffects.None,
                    0);
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Clean the current chunk's texture.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private void Clean(object sender, GameTime arg)
        {
            _graphics.SetRenderTarget(_texture);

            _effect.View = Matrix.Identity;
            _effect.Projection = Matrix.CreateOrthographicOffCenter(_position.X, _position.X + Chunk.Size, _position.Y + Chunk.Size, _position.Y, 0, 1);

            _spriteBatch.Begin(effect: _effect);
            this.driven.GetSurrounding().ForEach(c =>
            {
                c.Components.TryUpdateAll(arg);
                c.Components.TryDrawAll(arg);
            });
            _spriteBatch.End();


            // Reset graphics device
            _graphics.SetRenderTarget(null);

        }
        #endregion
    }
}
