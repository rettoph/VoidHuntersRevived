using Guppy.Extensions.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Chunks;

namespace VoidHuntersRevived.Client.Library.Components.Entities.Chunks
{
    internal sealed class ChunkDrawComponent : FrameableDrawComponent<Chunk>
    {
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.primitiveBatch.TraceRectangle(Color.Red, new Rectangle(
                this.Entity.Position.X * Chunk.Size,
                this.Entity.Position.Y * Chunk.Size,
                Chunk.Size,
                Chunk.Size));
        }
    }
}
