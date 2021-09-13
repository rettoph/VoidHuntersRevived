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
using VoidHuntersRevived.Library.Interfaces;
using VoidHuntersRevived.Library.Structs;

namespace VoidHuntersRevived.Library.Components.Entities.WorldObjects
{
    internal sealed class WorldObjectChunkComponent : Component<IWorldObject>
    {
        #region Private Fields
        private ChunkManager _chunks;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _chunks);

            this.Entity.OnPostUpdate += this.PostUpdate;
        }

        protected override void Initialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);

            // Automatically add the current entity into its appropriate chunk.
            _chunks.GetChunk(this.Entity.Position).Children.TryAdd(this.Entity);
        }

        protected override void PostRelease()
        {
            base.PostRelease();

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
    }
}
