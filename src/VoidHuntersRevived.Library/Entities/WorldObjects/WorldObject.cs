using Guppy;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.Interfaces;
using Guppy.Network;
using Guppy.Network.Enums;
using Guppy.Network.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Chunks;
using VoidHuntersRevived.Library.Interfaces;

namespace VoidHuntersRevived.Library.Entities.WorldObjects
{
    public abstract class WorldObject : MagicNetworkFrameable, IWorldObject
    {
        #region Private Fields
        private Chunk _chunk;
        #endregion

        #region Public Properties
        public abstract Vector2 Position { get; }
        public abstract Single Rotation { get; }

        public Chunk Chunk
        {
            get => _chunk;
        }
        Chunk IWorldObject.Chunk
        {
            get => _chunk;
            set => this.OnChunkChanged.InvokeIf(_chunk != value, this, ref _chunk, value);
        }
        #endregion

        #region Events
        public event OnChangedEventDelegate<IWorldObject, Chunk> OnChunkChanged;
        #endregion

        #region Helper Methods
        public abstract void SetTransform(Vector2 position, Single rotation);
        #endregion
    }
}
