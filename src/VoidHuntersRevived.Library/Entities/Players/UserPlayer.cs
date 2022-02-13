using Guppy.EntityComponent.DependencyInjection;
using Guppy.Network.Enums;
using Guppy.Network.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using VoidHuntersRevived.Library.Entities.Chunks;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Interfaces;

namespace VoidHuntersRevived.Library.Entities.Players
{
    public class UserPlayer : Player
    {
        #region Private Fields
        private ChunkManager _chunks;
        private IEnumerable<Chunk> _proximityChunks;
        #endregion

        #region Public Properties
        public User User { get; internal set; }
        public Int32 ChunkProximityRadius { get; set; } = 4;
        public Boolean ChunkLoader { get; private set; }
        public Boolean IsCurrentUser => this.User?.IsCurrentUser ?? true;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _chunks);

            this.OnShipChanged += this.HandleShipChanged;
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            this.Pipe.Users.TryAdd(this.User);

            this.ChunkLoader = this.IsCurrentUser || provider.Settings.Get<NetworkAuthorization>() == NetworkAuthorization.Master;
        }

        protected override void Uninitialize()
        {
            base.Uninitialize();

            this.Pipe.Users.TryRemove(this.User);
        }

        protected override void PostUninitialize()
        {
            base.PostUninitialize();

            this.OnShipChanged -= this.HandleShipChanged;
        }
        #endregion

        #region Helper Methods
        private void CleanChunkDependents(Chunk old, Chunk value)
        {
            if (!this.ChunkLoader)
                return;

            IEnumerable<Chunk> oldProximityChunks = _proximityChunks ?? _chunks.GetChunks(old?.Position, this.ChunkProximityRadius);
            _proximityChunks = _chunks.GetChunks(value?.Position, this.ChunkProximityRadius);

            // Deregister any old chunk dependents...
            foreach (Chunk chunk in oldProximityChunks.Except(_proximityChunks).OrderByDescending(chunk => chunk.Position.Distance(value.Position)))
            {
                chunk.Pipe.Users.TryRemove(this.User);
                chunk.TryDeregisterDependent(this.Id);
            }

            // Register any new chunk dependents...
            foreach (Chunk chunk in _proximityChunks.Except(oldProximityChunks).OrderBy(chunk => chunk.Position.Distance(value.Position)))
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
