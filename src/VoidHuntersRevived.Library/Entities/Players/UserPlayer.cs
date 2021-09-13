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
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Interfaces;
using VoidHuntersRevived.Library.Structs;

namespace VoidHuntersRevived.Library.Entities.Players
{
    public class UserPlayer : Player
    {
        #region Private Fields
        private ChunkManager _chunks;
        private IEnumerable<Chunk> _proximityChunks;
        #endregion

        #region Public Properties
        public IUser User { get; set; }
        public Int32 ChunkProximityRadius { get; set; } = 5;
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

            this.OnShipChanged += this.HandleShipChanged;
        }
        protected override void Release()
        {
            base.PreRelease();

            this.Ship = default;

            this.OnShipChanged -= this.HandleShipChanged;

            _chunks = default;
        }

        protected override void Dispose()
        {
            base.Dispose();

            // this.OnChunkChanged -= this.HandleChunkChanged;
        }
        #endregion

        #region Helper Methods
        private void CleanChunkDependents(Chunk old, Chunk value)
        {
            IEnumerable<Chunk> oldProximityChunks = _proximityChunks ?? _chunks.GetChunks(old?.Position, this.ChunkProximityRadius);
            _proximityChunks = _chunks.GetChunks(value?.Position, this.ChunkProximityRadius);

            // Deregister any old chunk dependents...
            foreach (Chunk chunk in oldProximityChunks.Except(_proximityChunks))
            {
                chunk.Pipe.Users.TryRemove(this.User);
                chunk.TryDeregisterDependent(this.Id);
            }


            // Register any new chunk dependents...
            foreach (Chunk chunk in _proximityChunks.Except(oldProximityChunks))
            {
                chunk.Pipe.Users.TryAdd(this.User);
                chunk.TryRegisterDependent(this.Id);
            }
        }
        #endregion

        #region Event Handlers
        private void HandleShipChanged(Player sender, Ship old, Ship value)
        {
            if(old != default)
            {
                old.Chain.OnChunkChanged -= this.HandlePlayerShipChainChunkChained;
            }
            
            if(value != default)
            {
                value.Chain.OnChunkChanged += this.HandlePlayerShipChainChunkChained;
            }

            this.CleanChunkDependents(old?.Chain.Chunk, value?.Chain.Chunk);
        }

        private void HandlePlayerShipChainChunkChained(IWorldObject sender, Chunk old, Chunk value)
        {
            this.CleanChunkDependents(old, value);
        }
        #endregion
    }
}
