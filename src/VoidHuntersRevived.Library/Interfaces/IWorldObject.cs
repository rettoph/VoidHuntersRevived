using Guppy.Interfaces;
using Guppy.Network.Interfaces;
using Microsoft.Xna.Framework;
using System;
using VoidHuntersRevived.Library.Entities.Chunks;

namespace VoidHuntersRevived.Library.Interfaces
{
    /// <summary>
    /// World objects represent objects with a physical location.
    /// The exist within a chunk & may be serialized as needed.
    /// </summary>
    public interface IWorldObject : IMagicNetworkEntity, IFrameable
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
        #endregion

        #region Events
        event OnChangedEventDelegate<IWorldObject, Chunk> OnChunkChanged;
        #endregion

        #region Methods
        /// <summary>
        /// Transform the current instance's <see cref="Position"/>
        /// and <see cref="Rotation"/>.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        void SetTransform(Vector2 position, Single rotation);
        #endregion
    }
}
