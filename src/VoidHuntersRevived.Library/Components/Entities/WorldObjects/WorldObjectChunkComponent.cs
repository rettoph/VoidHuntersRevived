using Guppy;
using Guppy.DependencyInjection;
using Guppy.Enums;
using Guppy.Extensions.DependencyInjection;
using Guppy.Interfaces;
using Guppy.Network.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Chunks;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Structs;

namespace VoidHuntersRevived.Library.Components.Entities.WorldObjects
{
    internal sealed class WorldObjectChunkComponent : Component<IWorldObject>
    {
        #region Private Fields
        private ChunkManager _chunks;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);

            provider.Service(out _chunks);

            this.Entity.OnStatus[ServiceStatus.Initializing] += this.HandleEntityInitializing;
            this.Entity.OnStatus[ServiceStatus.Releasing] += this.HandleEntityReleasing;
            this.Entity.OnPostUpdate += this.PostUpdate;
        }

        protected override void Release()
        {
            base.Release();

            this.Entity.OnStatus[ServiceStatus.Initializing] -= this.HandleEntityInitializing;
            this.Entity.OnStatus[ServiceStatus.Releasing] -= this.HandleEntityReleasing;
            this.Entity.OnPostUpdate -= this.PostUpdate;

            _chunks = default;
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
                chunk.Children.TryAdd(this.Entity);
            }
        }
        #endregion

        #region Frame Methods
        private void PostUpdate(GameTime gameTime)
        {
            this.CleanChunk();
        }
        #endregion

        #region Event Handlers
        private void HandleEntityInitializing(IService sender, ServiceStatus old, ServiceStatus value)
        {
            this.Entity.OnChunkChanged += this.HandleWorldObjectChunkChanged;

            // Automatically add the current entity into its appropriate chunk.
            _chunks.GetChunk(this.Entity.Position).Children.TryAdd(this.Entity);
        }

        private void HandleEntityReleasing(IService sender, ServiceStatus old, ServiceStatus value)
        {
            this.Entity.OnChunkChanged -= this.HandleWorldObjectChunkChanged;

            this.Entity.Chunk?.Children.TryRemove(this.Entity);
            this.Entity.Chunk = default;
        }


        private void HandleWorldObjectChunkChanged(IWorldObject sender, Chunk old, Chunk value)
        {
            if(old != default)
            {
                old.OnStatus[ServiceStatus.Releasing] -= this.HandleChunkReleasing;
            }

            if(value != default)
            {
                value.OnStatus[ServiceStatus.Releasing] += this.HandleChunkReleasing;
            }
        }

        private void HandleChunkReleasing(IService sender, ServiceStatus old, ServiceStatus value)
            => this.Entity.TryRelease();
        #endregion
    }
}
