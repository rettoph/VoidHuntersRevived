using Guppy;
using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.Network;
using Guppy.Network.Enums;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Chunks;

namespace VoidHuntersRevived.Library.Entities.WorldObjects
{
    public abstract class WorldObject : NetworkLayerable, IWorldObject
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

        /// <inheritdoc />
        public event OnEventDelegate<IWorldObject> OnWorldInfoChangeDetected;

        /// <inheritdoc />
        public event ValidateEventDelegate<IWorldObject, GameTime> ValidateWorldInfoChangeDetected;
        #endregion

        #region Helper Methods
        /// <inheritdoc />
        void IWorldObject.TryValidateWorldInfoChanged(GameTime gameTime)
        {
            if(this.ValidateWorldInfoChangeDetected.Validate(this, gameTime, false))
            {
                this.OnWorldInfoChangeDetected?.Invoke(this);
            }
        }

        /// <inheritdoc />
        public abstract void TrySetTransformation(Vector2 position, Single rotation, NetworkAuthorization authorization = NetworkAuthorization.Master);
        #endregion
    }
}
