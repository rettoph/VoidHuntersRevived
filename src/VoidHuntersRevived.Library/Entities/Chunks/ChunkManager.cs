using Guppy;
using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.Extensions.DependencyInjection;
using Guppy.Lists;
using Guppy.Lists.Delegates;
using Guppy.Lists.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Structs;

namespace VoidHuntersRevived.Library.Entities.Chunks
{
    /// <summary>
    /// Simple helper class used for the management of 
    /// chunks on a local scale.
    /// </summary>
    public class ChunkManager : Layerable
    {
        #region Private Fields
        private FrameableList<Chunk> _chunks;
        #endregion

        #region Events
        public event OnEventDelegate<IServiceList<Chunk>, Chunk> OnChunkAdded
        {
            add => _chunks.OnAdded += value;
            remove => _chunks.OnAdded -= value;
        }

        public event OnEventDelegate<IServiceList<Chunk>, Chunk> OnChunkRemoved
        {
            add => _chunks.OnRemoved += value;
            remove => _chunks.OnRemoved -= value;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Create(GuppyServiceProvider provider)
        {
            base.Create(provider);

            this.LayerGroup = Constants.LayersContexts.Chunks.Group.GetValue();
        }

        protected override void Initialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);

            provider.Service(out _chunks);
        }

        protected override void Release()
        {
            base.Release();

            _chunks.TryRelease();

            _chunks = default;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _chunks.TryUpdate(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _chunks.TryDraw(gameTime);
        }
        #endregion

        #region Helper Methods
        public Chunk GetChunk(ChunkPosition position)
        {
            return _chunks.GetOrCreateById<Chunk>(position.Id);
        }

        public IEnumerable<Chunk> GetChunks(ChunkPosition position, Int32 radius = 1)
        {
            for (Int32 x = -radius; x <= radius; x++)
            {
                for (Int32 y = -radius; y <= radius; y++)
                {
                    yield return _chunks.GetOrCreateById<Chunk>((new ChunkPosition(position.X + x, position.Y + y)).Id);
                }
            }
        }
        #endregion
    }
}
