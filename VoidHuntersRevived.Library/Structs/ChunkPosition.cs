using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using VoidHuntersRevived.Library.Entities.Controllers;

namespace VoidHuntersRevived.Library.Structs
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ChunkPosition
    {
        [FieldOffset(0)]
        private Guid _id;
        [FieldOffset(0)]
        public readonly Single X;
        [FieldOffset(4)]
        public readonly Single Y;
        public Guid Id { get => _id; }

        public ChunkPosition(Single x, Single y)
        {
            _id = Guid.Empty;
            this.X = (Single)Math.Floor(x / Chunk.Size) * Chunk.Size;
            this.Y = (Single)Math.Floor(y / Chunk.Size) * Chunk.Size;
        }
    }
}
