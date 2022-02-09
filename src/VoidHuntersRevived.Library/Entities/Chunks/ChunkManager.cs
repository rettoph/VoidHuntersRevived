using Guppy;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.EntityComponent.Lists;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using VoidHuntersRevived.Library.Globals.Constants;
using VoidHuntersRevived.Library.Structs;
using Guppy.EntityComponent.Lists.Interfaces;

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
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.LayerGroup = LayersContexts.Chunks.Group.GetValue();
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            provider.Service(out _chunks);
        }

        protected override void Uninitialize()
        {
            base.Uninitialize();

            _chunks.Dispose();
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
        public Chunk GetChunk(Vector2 worldPosition)
        {
            return this.GetChunk(new ChunkPosition(worldPosition));
        }
        public Chunk GetChunk(ChunkPosition position)
        {
            if(_chunks.TryGetById(position.Id, out Chunk chunk))
            {
                return chunk;
            }

            // We need to create & return a new chunk here
            return _chunks.Create((chunk, _, _) =>
            {
                chunk.Position = position;
            });
        }

        public IEnumerable<Chunk> GetChunks(ChunkPosition? position, Int32 radius = 1)
        {
            if (position != default)
            {
                for (Int32 x = -radius; x <= radius; x++)
                {
                    for (Int32 y = -radius; y <= radius; y++)
                    {
                        yield return this.GetChunk(new ChunkPosition(position.Value.X + x, position.Value.Y + y));
                    }
                }
            }
        }
        #endregion
    }
}
