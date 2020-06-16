using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.Collections;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Entities.Controllers
{
    public class Chunk : Frameable
    {
        #region Structs
        [StructLayout(LayoutKind.Explicit)]
        public struct Position
        {
            [FieldOffset(0)]
            private Guid _id;
            [FieldOffset(0)]
            public readonly Double X;
            [FieldOffset(8)]
            public readonly Double Y;

            public Guid Id => _id;

            public Position(Single x, Single y)
            {
                _id = Guid.Empty;
                this.X = Math.Floor(x / Chunk.Size);
                this.Y = Math.Floor(y / Chunk.Size);
            }
            public Position(Vector2 pos) : this(pos.X, pos.Y)
            {

            }
            public Position(Vector3 pos) : this(pos.X, pos.Y)
            {

            }
        }
        #endregion

        #region Static Attributes
        public static Int32 Size { get; } = 64;
        public static Vertices Vertices { get; } = new Vertices(new Vector2[] { new Vector2(0, 0), new Vector2(0, Chunk.Size), new Vector2(Chunk.Size, Chunk.Size), new Vector2(Chunk.Size, 0) });
        #endregion

        #region Private Fields
        private Position _position;
        private WorldEntity _world;
        private HashSet<ShipPart> _parts;
        private ChunkManager _chunks;
        #endregion

        #region Public Attributes
        public override Guid Id { 
            get => _position.Id;
            set {
                // Do nothing...
            }
        }

        public Single X { get; private set; }
        public Single Y { get; private set; }

        public Rectangle Bounds { get; private set; }
        #endregion

        #region Lifecycle Methids
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _parts = new HashSet<ShipPart>();

            provider.Service(out _world);
            provider.Service(out _chunks);
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _parts.ForEach(p => _chunks.shipPartChunks[p].TryDraw(gameTime));
        }
        #endregion

        #region Helper Methods
        internal virtual void SetPosition(Position position)
        {
            _position = position;

            this.X = (Single)_position.X * Chunk.Size;
            this.Y = (Single)_position.Y * Chunk.Size;
            this.Bounds = new Rectangle((Int32)this.X, (Int32)this.Y, Chunk.Size, Chunk.Size);
        }

        internal void Add(ShipPart shipPart)
            => _parts.Add(shipPart);

        internal void Remove(ShipPart shipPart)
            => _parts.Remove(shipPart);
        #endregion
    }
}
