using Guppy;
using Guppy.EntityComponent;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.Interfaces;
using Guppy.Network.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Chunks;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Interfaces;
using VoidHuntersRevived.Library.Structs;

namespace VoidHuntersRevived.Library.Components.WorldObjects
{
    internal sealed class WorldObjectChunkComponent : Component<IWorldObject>
    {
        #region Private Fields
        private ChunkManager _chunks;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _chunks);

            this.Entity.OnChunkChanged += this.HandleChunkChanged;
            this.Entity.OnPostUpdate += this.PostUpdate;
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            // Automatically add the current entity into its appropriate chunk.
            this.Entity.Chunk = _chunks.GetChunk(this.Entity.Position);
        }

        protected override void Uninitialize()
        {
            base.Uninitialize();

            this.Entity.Chunk = default;
        }

        protected override void PostUninitialize()
        {
            base.PostUninitialize();

            this.Entity.OnPostUpdate -= this.PostUpdate;
            this.Entity.OnChunkChanged -= this.HandleChunkChanged;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Check to see if the item is still residing within its current chunk.
        /// </summary>
        private void CleanChunk()
        {
            if (!this.Entity.Chunk.Bounds.Contains(this.Entity.Position))
            {
                Chunk chunk = _chunks.GetChunk(this.Entity.Position);

                this.Entity.Chunk = chunk;
            }
        }
        #endregion

        #region Frame Methods
        private void PostUpdate(GameTime gameTime)
        {
            this.CleanChunk();
        }
        #endregion

        #region Event Handler 
        private void HandleChunkChanged(IWorldObject sender, Chunk old, Chunk value)
        {
            if(old is not null)
            {
                old.Children.TryRemove(this.Entity);
            }
            
            if(value is not null)
            {
                this.Entity.Pipe = value.Pipe;
                value.Children.TryAdd(this.Entity);
            }
        }
        #endregion
    }
}
