using Guppy.Collections;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Structs;

namespace VoidHuntersRevived.Library.Collections
{
    /// <summary>
    /// The primary source of chunk management.
    /// 
    /// This allows for the selection of a chunk based
    /// on recieved x/y coordinates.
    /// </summary>
    public sealed class ChunkCollection : FrameableCollection<Chunk>
    {
        #region Private Fields
        private EntityCollection _entities;
        #endregion

        #region Contructors
        public ChunkCollection(EntityCollection entities, IServiceProvider provider) : base(provider)
        {
            _entities = entities;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Return the chunk that contains the given x and y position.
        /// 
        /// This will create a new chunk if one does not already exist.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="create">If the chunk should be created if it doesnt already exist</param>
        /// <returns></returns>
        public Chunk Get(Single x, Single y, Boolean create = true)
        {
            Chunk output;
            ChunkPosition position = new ChunkPosition(x, y);

            if((output = this.GetById(position.Id)) == default(Chunk) && create)
            { // Create a brand new chunk
                output = _entities.Create<Chunk>("entity:chunk", c =>
                {
                    c.SetId(position.Id);
                    c.Position = position;

                    // Add the new chunk to the internal collection
                    this.Add(c);
                }); 
            }

            return output;
        }

        /// <summary>
        /// Return the chunk contains the given entities position.
        /// 
        /// This will create a new chunk if one does not already exist.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Chunk Get(FarseerEntity entity)
        {
            return this.Get(entity.Position.X, entity.Position.Y);
        }

        /// <summary>
        /// Automatically add a farseer entity into its
        /// appropriate chunk.
        /// </summary>
        /// <param name="entity"></param>
        public void AddToChunk(FarseerEntity entity)
        {
            this.Get(entity).Add(entity);
        }
        #endregion
    }
}
