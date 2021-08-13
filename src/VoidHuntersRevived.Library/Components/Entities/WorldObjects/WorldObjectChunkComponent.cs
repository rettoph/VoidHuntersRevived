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
        }


        protected override void Release()
        {
            base.Release();

            this.Entity.OnStatus[ServiceStatus.Initializing] -= this.HandleEntityInitializing;
            this.Entity.OnStatus[ServiceStatus.Releasing] -= this.HandleEntityReleasing;

            _chunks = default;
        }
        #endregion

        #region Helper Methods
        private void CleanChunk()
        {
            ChunkPosition chunkPosition = new ChunkPosition(this.Entity.Position);

            // TODO: Remove this null check somehow
            if (this.Entity.Chunk?.Id != chunkPosition.Id)
            {
                Chunk chunk = _chunks.GetChunk(chunkPosition);
                chunk.Children.TryAdd(this.Entity);
            }
        }
        #endregion

        #region Event Handlers
        private void HandleEntityInitializing(IService sender, ServiceStatus old, ServiceStatus value)
        {
            this.Entity.OnWorldInfoChangeDetected += this.HandleWorldObjectWorldInfoChangeChanged;
            this.Entity.OnChunkChanged += this.HandleWorldObjectChunkChanged;

            this.CleanChunk();
        }

        private void HandleEntityReleasing(IService sender, ServiceStatus old, ServiceStatus value)
        {
            this.Entity.OnWorldInfoChangeDetected -= this.HandleWorldObjectWorldInfoChangeChanged;
            this.Entity.OnChunkChanged -= this.HandleWorldObjectChunkChanged;

            this.Entity.Chunk = default;
        }

        private void HandleWorldObjectWorldInfoChangeChanged(IWorldObject sender)
            => this.CleanChunk();


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
