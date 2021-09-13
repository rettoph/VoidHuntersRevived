using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Extensions.Utilities;
using Guppy.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Chunks;

namespace VoidHuntersRevived.Client.Library.Components.Entities.Chunks
{
    internal sealed class ChunkDrawComponent : FrameableDrawComponent<Chunk>
    {
        private SpriteFont _font;
        private Camera2D _camera;

        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _camera);

            _font = provider.GetContent<SpriteFont>("guppy:font:debug");
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.spriteBatch.DrawString(
                _font, 
                $"({this.Entity.Position.X}, {this.Entity.Position.Y})", 
                this.Entity.Bounds.Location.ToVector2(), 
                Color.Red, 
                0f, 
                Vector2.Zero, 
                1f / _camera.Zoom, 
                SpriteEffects.None, 
                0);

            this.primitiveBatch.TraceRectangle(Color.Red, new Rectangle(
                this.Entity.Position.X * Chunk.Size,
                this.Entity.Position.Y * Chunk.Size,
                Chunk.Size,
                Chunk.Size));
        }
    }
}
