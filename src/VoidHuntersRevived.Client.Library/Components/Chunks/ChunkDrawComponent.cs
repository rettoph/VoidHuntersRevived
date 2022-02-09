using Guppy.CommandLine.Services;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.Threading.Interfaces;
using Guppy.Utilities;
using Guppy.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Messages.Commands;
using VoidHuntersRevived.Library.Entities.Chunks;
using VoidHuntersRevived.Library.Interfaces;

namespace VoidHuntersRevived.Client.Library.Components.Chunks
{
    internal sealed class ChunkDrawComponent : FrameableDrawComponent<Chunk>,
        IDataProcessor<ToggleRenderChunkDebugViewCommand>
    {
        private Boolean _gridVisible;
        private SpriteFont _font;
        private CommandService _commands;

        public Boolean GridVisible
        {
            get => _gridVisible;
            set
            {
                if(_gridVisible == value)
                {
                    return;
                }

                if (_gridVisible = value)
                {
                    this.Entity.OnPostDraw += this.DrawGrid;
                }
                else
                {
                    this.Entity.OnPostDraw -= this.DrawGrid;
                }
            }
        }

        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _commands);

            _commands.RegisterProcessor<ToggleRenderChunkDebugViewCommand>(this);
            _font = provider.GetContent<SpriteFont>("guppy:font:debug");
        }

        protected override void Uninitialize()
        {
            base.Uninitialize();

            _commands.DeregisterProcessor<ToggleRenderChunkDebugViewCommand>(this);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.camera.Frustum.Intersects(ref this.Entity.BoundingBox, out bool contains);

            if (contains)
            {
                this.Entity.Children.TryDraw(gameTime);
            }
        }

        private void DrawGrid(GameTime gameTime)
        {
            this.spriteBatch.DrawString(
                _font, 
                $"({this.Entity.Position.X}, {this.Entity.Position.Y})", 
                this.Entity.Bounds.Location.ToVector2(), 
                Color.Red, 
                0f, 
                Vector2.Zero, 
                1f / this.camera.Zoom, 
                SpriteEffects.None, 
                0);
            
            this.primitiveBatch.TraceRectangle(Color.Red, new Rectangle(
                this.Entity.Position.X * Chunk.Size,
                this.Entity.Position.Y * Chunk.Size,
                Chunk.Size,
                Chunk.Size));
        }

        bool IDataProcessor<ToggleRenderChunkDebugViewCommand>.Process(ToggleRenderChunkDebugViewCommand data)
        {
            this.GridVisible = !this.GridVisible;

            return true;
        }
    }
}
