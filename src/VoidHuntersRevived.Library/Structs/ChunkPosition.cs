using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using VoidHuntersRevived.Library.Entities.Chunks;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Structs
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ChunkPosition
    {
        [FieldOffset(0)]
        private PipeType _pipeType;

        [FieldOffset(0)]
        public Guid Id;

        [FieldOffset(1)]
        public Int32 X;

        [FieldOffset(9)]
        public Int32 Y;

        #region Constructors
        /// <summary>
        /// Convert world coordinates to chunk coordinates
        /// </summary>
        /// <param name="x">World X</param>
        /// <param name="y">World Y</param>
        public ChunkPosition(Single x, Single y)
        {
            _pipeType = PipeType.Chunk;

            this.Id = default;
            this.X = (Int32)Math.Floor(x / Chunk.Size);
            this.Y = (Int32)Math.Floor(y / Chunk.Size);
        }

        /// <summary>
        /// Convert world coordinates to chunk coordinates
        /// </summary>
        /// <param name="pos">World coords</param>
        public ChunkPosition(Vector2 pos) : this(pos.X, pos.Y)
        {

        }

        /// <summary>
        /// Convert world coordinates to chunk coordinates
        /// </summary>
        /// <param name="pos">World coords</param>
        public ChunkPosition(Vector3 pos) : this(pos.X, pos.Y)
        {

        }

        public ChunkPosition(Guid id) : this()
        {
            _pipeType = PipeType.Chunk;

            this.Id = id;
        }

        /// <summary>
        /// Create a ChunkPosition from ChunkCoordinates
        /// </summary>
        /// <param name="x">Chunk X</param>
        /// <param name="y">Chunk Y</param>
        public ChunkPosition(Int32 x, Int32 y)
        {
            _pipeType = PipeType.Chunk;

            this.Id = default;
            this.X = x;
            this.Y = y;
        }
        #endregion

        #region Implicit Operators
        public static implicit operator ChunkPosition(Vector2 pos) => new ChunkPosition(pos);
        public static implicit operator ChunkPosition(Vector3 pos) => new ChunkPosition(pos);
        #endregion

        #region Equal Operators
        public static bool operator ==(ChunkPosition cp1, ChunkPosition cp2)
            => cp1.X == cp2.X && cp1.Y == cp2.Y;

        public static bool operator !=(ChunkPosition cp1, ChunkPosition cp2) => !(cp1 == cp2);

        public override bool Equals(object obj)
        {
            if (obj is ChunkPosition cp)
                return this == cp;

            return false;
        }

        public Double Distance(ChunkPosition position)
        {
            return Math.Sqrt(Math.Pow(this.X - position.X, 2) + Math.Pow(this.Y - position.Y, 2));
        }

        public override int GetHashCode()
        {
            int hashCode = 1809447997;
            hashCode = hashCode * -1521134295 + _pipeType.GetHashCode();
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }
        #endregion
    }
}
