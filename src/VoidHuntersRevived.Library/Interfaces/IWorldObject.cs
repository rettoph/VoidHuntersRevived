using Guppy.Events.Delegates;
using Guppy.Interfaces;
using Guppy.Network.Enums;
using Guppy.Network.Interfaces;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using VoidHuntersRevived.Library.Entities.Chunks;

namespace VoidHuntersRevived.Library.Interfaces
{
    /// <summary>
    /// World objects represent objects with a physical location.
    /// The exist within a chunk & may be serialized as needed.
    /// </summary>
    public interface IWorldObject : ILayerable, INetworkEntity
    {
        #region Public Properties
        /// <summary>
        /// The current instance's position.
        /// </summary>
        Vector2 Position { get; }

        /// <summary>
        /// The current instance's rotation.
        /// </summary>
        Single Rotation { get; }

        /// <summary>
        /// The object's current chunk. This will automatically & 
        /// dynamically be managed via the <see cref="WorldObjectChunkComponent"/>
        /// </summary>
        Chunk Chunk { get; internal set; }

        /// <summary>
        /// Indicates the current IWorldObject's world info is in a dirty state.
        /// This is only relevant for networking purposes.
        /// </summary>
        Boolean WorldInfoDirty { get; set; }

        /// <summary>
        /// Indicates the current IWorldObject should be
        /// brroadcasted through the network when dirty.
        /// </summary>
        Boolean Sleeping { get; set; }
        #endregion

        #region Events
        event OnChangedEventDelegate<IWorldObject, Chunk> OnChunkChanged;

        /// <summary>
        /// Invoked by <see cref="TryValidateWorldInfoDirty"/> when the
        /// <see cref="ValidateWorldInfoDirty"/> event returns a
        /// true state.
        /// </summary>
        event OnEventDelegate<IWorldObject, Boolean> OnWorldInfoDirtyChanged;

        /// <summary>
        /// Determin whether or not any world info data has changed. This
        /// should be invoked by <see cref="TryValidateWorldInfoDirty"/>.
        /// </summary>
        event ValidateEventDelegate<IWorldObject, GameTime> ValidateWorldInfoDirty;
        #endregion

        #region Methods
        /// <summary>
        /// Should invoke the <see cref="OnWorldInfoDirtyChanged"/> event
        /// if the internal world data has been updated.
        /// </summary>
        /// <returns></returns>
        void TryValidateWorldInfoDirty(GameTime gameTime);
        #endregion

        #region Network Methods
        public void WriteWorldInfo(NetOutgoingMessage om);

        public void ReadWorldInfo(NetIncomingMessage im);
        #endregion
    }
}
