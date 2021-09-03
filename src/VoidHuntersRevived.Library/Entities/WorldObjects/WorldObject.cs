using Guppy;
using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.Network;
using Guppy.Network.Enums;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
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
        private Boolean _worldInfoDirty;
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

        public Boolean WorldInfoDirty
        {
            get => _worldInfoDirty;
            set => this.OnWorldInfoDirtyChanged.InvokeIf(_worldInfoDirty != value, this, ref _worldInfoDirty, value);
        }
        #endregion

        #region Events
        public event OnChangedEventDelegate<IWorldObject, Chunk> OnChunkChanged;

        /// <inheritdoc />
        public event OnEventDelegate<IWorldObject, Boolean> OnWorldInfoDirtyChanged;

        /// <inheritdoc />
        public event ValidateEventDelegate<IWorldObject, GameTime> ValidateWorldInfoDirty;
        #endregion

        #region Helper Methods
        /// <inheritdoc />
        void IWorldObject.TryValidateWorldInfoDirty(GameTime gameTime)
        {
            if(!_worldInfoDirty && this.ValidateWorldInfoDirty.Validate(this, gameTime, false))
            {
                this.WorldInfoDirty = true;
            }
        }
        #endregion

        #region Network Methods
        void IWorldObject.WriteWorldInfo(NetOutgoingMessage om)
            => this.WriteWorldInfo(om);

        void IWorldObject.ReadWorldInfo(NetIncomingMessage im)
            => this.ReadWorldInfo(im);

        protected abstract void WriteWorldInfo(NetOutgoingMessage om);

        protected abstract void ReadWorldInfo(NetIncomingMessage im);
        #endregion
    }
}
