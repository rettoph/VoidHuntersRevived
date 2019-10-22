using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Guppy;
using Guppy.Extensions.Collection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Library.Collections;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Utilities.Controllers;

namespace VoidHuntersRevived.Library.Utilities
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Position
    {
        [FieldOffset(0)]
        private Guid _id;
        [FieldOffset(0)]
        public Single X;
        [FieldOffset(4)]
        public Single Y;
        public Guid Id { get => _id; }
    }

    public class Chunk : Controller<FarseerEntity>
    {
        #region Static Attributes
        public static Single Size { get; private set; } = 32;
        #endregion

        #region Private Attributes
        private ChunkCollection _chunks;
        private IEnumerable<Chunk> _surrounding;
        private GameTime _addedTime;
        #endregion

        #region Public Attributes
        public Position Position { get; internal set; }
        public Boolean Dirty { get; private set; }
        #endregion

        #region Constructor
        public Chunk(ChunkCollection chunks)
        {
            _chunks = chunks;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _addedTime = new GameTime();

            this.CollidesWith = Categories.PassiveCollidesWith;
            this.CollisionCategories = Categories.PassiveCollisionCategories;
            this.IgnoreCCDWith = Categories.PassiveIgnoreCCDWith;

            this.Events.Register<GameTime>("cleaned");
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            if (this.Dirty)
            {
                this.Events.TryInvoke<GameTime>(this, "cleaned", gameTime);
                this.Dirty = false;
            }

            base.Update(gameTime);
        }
        #endregion

        #region Controller Overrides
        public override bool Add(FarseerEntity entity)
        {
            if(base.Add(entity))
            {
                entity.Events.TryAdd<Creatable>("disposing", this.HandleEntityDisposing);

                // Update the entity once
                entity.TryUpdate(_addedTime);

                this.MarkDirty();

                return true;
            }

            return false;
        }

        protected override bool Remove(FarseerEntity entity)
        {
            if (base.Remove(entity))
            {
                entity.Events.TryRemove<Creatable>("disposing", this.HandleEntityDisposing);

                this.MarkDirty();

                return true;
            }

            return false;
        }
        #endregion

        #region Helper Methods
        private void MarkDirty()
        {
            this.Dirty = true;
            this.GetSurrounding().ForEach(c => {
                c.Dirty = true;
            });
        }

        /// <summary>
        /// Get all chunks surrounding the current chunk.
        /// This will create the chunks if they do not exist
        /// This does not include the current chunk
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Chunk> GetOrCreateSurrounding()
        {
            if (_surrounding == default(IEnumerable<Chunk>))
            {
                var list = new List<Chunk>();
                list.Add(_chunks.GetOrCreate(this.Position.X + Chunk.Size, this.Position.Y + Chunk.Size));
                list.Add(_chunks.GetOrCreate(this.Position.X + 0, this.Position.Y + Chunk.Size));
                list.Add(_chunks.GetOrCreate(this.Position.X - Chunk.Size, this.Position.Y + Chunk.Size));

                list.Add(_chunks.GetOrCreate(this.Position.X - Chunk.Size, this.Position.Y + 0));
                list.Add(_chunks.GetOrCreate(this.Position.X + Chunk.Size, this.Position.Y + 0));

                list.Add(_chunks.GetOrCreate(this.Position.X + Chunk.Size, this.Position.Y - Chunk.Size));
                list.Add(_chunks.GetOrCreate(this.Position.X + 0, this.Position.Y - Chunk.Size));
                list.Add(_chunks.GetOrCreate(this.Position.X - Chunk.Size, this.Position.Y - Chunk.Size));

                _surrounding = list;
            }

            return _surrounding;
        }

        /// <summary>
        /// Get all chunks surrounding the current chunk.
        /// This does not include the current chunk
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Chunk> GetSurrounding()
        {
            if (_surrounding == default(IEnumerable<Chunk>))
            {
                var list = new List<Chunk>();
                Chunk item;

                if ((item = _chunks.Get(this.Position.X + Chunk.Size, this.Position.Y + Chunk.Size)) != default(Chunk))
                    list.Add(item);
                if ((item = _chunks.Get(this.Position.X + 0, this.Position.Y + Chunk.Size)) != default(Chunk))
                    list.Add(item);
                if ((item = _chunks.Get(this.Position.X - Chunk.Size, this.Position.Y + Chunk.Size)) != default(Chunk))
                    list.Add(item);

                if ((item = _chunks.Get(this.Position.X - Chunk.Size, this.Position.Y + 0)) != default(Chunk))
                    list.Add(item);
                if ((item = _chunks.Get(this.Position.X + Chunk.Size, this.Position.Y + 0)) != default(Chunk))
                    list.Add(item);

                if ((item = _chunks.Get(this.Position.X + Chunk.Size, this.Position.Y - Chunk.Size)) != default(Chunk))
                    list.Add(item);
                if ((item = _chunks.Get(this.Position.X + 0, this.Position.Y - Chunk.Size)) != default(Chunk))
                    list.Add(item);
                if ((item = _chunks.Get(this.Position.X - Chunk.Size, this.Position.Y - Chunk.Size)) != default(Chunk))
                    list.Add(item);

                // Update the surrounding value, assuming that all chunks exist
                if (list.Count == 8)
                    _surrounding = list;
                else
                    return list;
            }

            return _surrounding;
        }
        #endregion

        private void HandleEntityDisposing(object sender, Creatable arg)
        {
            // auto remove any disposed entities
            this.Remove(arg as FarseerEntity);
        }
    }
}
