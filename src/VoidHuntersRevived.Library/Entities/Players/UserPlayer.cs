using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Network.Enums;
using Guppy.Network.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities.Chunks;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Structs;

namespace VoidHuntersRevived.Library.Entities.Players
{
    public class UserPlayer : Player
    {
        #region Private Fields
        private ChunkManager _chunks;
        private ChunkPosition _currentChunkPosition;
        #endregion

        #region Public Properties
        public IUser User { get; set; }
        public Int32 ChunkProximityRadius { get; set; } = 3;
        #endregion

        #region Lifecycle Methods
        protected override void Create(GuppyServiceProvider provider)
        {
            base.Create(provider);
        }

        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _chunks);
            _currentChunkPosition = new ChunkPosition(Vector2.Zero);
        }

        protected override void PostInitialize(GuppyServiceProvider provider)
        {
            base.PostInitialize(provider);

            foreach (Chunk chunk in _chunks.GetChunks(_currentChunkPosition, this.ChunkProximityRadius))
            {
                chunk.Pipe.Users.TryAdd(this.User);
                chunk.TryRegisterDependent(this.Id);
            }
                
        }

        protected override void PreRelease()
        {
            base.PreRelease();

            // Deregister any old chunk dependents...
            foreach (Chunk chunk in _chunks.GetChunks(_currentChunkPosition, this.ChunkProximityRadius))
            {
                chunk.Pipe.Users.TryRemove(this.User);
                chunk.TryDeregisterDependent(this.Id);
            }

            _chunks = default;
        }

        protected override void Dispose()
        {
            base.Dispose();

            // this.OnChunkChanged -= this.HandleChunkChanged;
        }
        #endregion

        #region Event Handlers
        // private void HandleChunkChanged(IWorldObject sender, Chunk old, Chunk value)
        // {
        //     var oldProximityChunks = _chunks.GetChunks(old?.Position, this.ChunkProximityRadius);
        //     var newProximityChunks = _chunks.GetChunks(value?.Position, this.ChunkProximityRadius);
        // 
        //     // Deregister any old chunk dependents...
        //     foreach (Chunk chunk in oldProximityChunks.Except(newProximityChunks))
        //     {
        //         chunk.Pipe.Users.TryRemove(this.User);
        //     }
        //         
        //     // Register any new chunk dependents...
        //     foreach (Chunk chunk in newProximityChunks.Except(oldProximityChunks))
        //     {
        //         chunk.Pipe.Users.TryAdd(this.User);
        //     }
        // }
        #endregion
    }
}
