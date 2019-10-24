using Guppy.Collections;
using Guppy.Extensions.Collection;
using Guppy.Factories;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Utilities;
using VoidHuntersRevived.Library.Utilities.Controllers;

namespace VoidHuntersRevived.Library.Collections
{
    public class ChunkCollection : FrameableCollection<Chunk>
    {
        #region Private Fields
        /// <summary>
        /// The main chunk factory.
        /// </summary>
        private DrivenFactory<Chunk> _factory;
        private Chunk _chunk;
        private Annex _annex;
        #endregion

        #region Constructor
        public ChunkCollection(Annex annex, DrivenFactory<Chunk> factory, IServiceProvider provider) : base(provider)
        {
            _factory = factory;
            _annex = annex;
        }
        #endregion

        #region GetOrCreate Methods
        public Chunk GetOrCreate(Single x, Single y)
        {
            var position = new Position()
            {
                X = (Single)Math.Floor(x / Chunk.Size) * Chunk.Size,
                Y = (Single)Math.Floor(y / Chunk.Size) * Chunk.Size
            };

            if((_chunk = this.GetById(position.Id)) == default(Chunk))
            { // If the chunk doesnt exist yet
                _chunk = _factory.Build<Chunk>(c =>
                {
                    c.SetId(position.Id);
                    c.Position = position;
                });

                this.Add(_chunk);
            }

            return _chunk;
        }

        public Chunk Get(Single x, Single y)
        {
            var position = new Position()
            {
                X = (Single)Math.Floor(x / Chunk.Size) * Chunk.Size,
                Y = (Single)Math.Floor(y / Chunk.Size) * Chunk.Size
            };

            return _chunk = this.GetById(position.Id);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Automatically add several entities to their respective chunks
        /// </summary>
        /// <param name="list"></param>
        public void AddMany(IEnumerable<FarseerEntity> list)
        {
            list.ForEach(f => {
                if (f.Status == Guppy.InitializationStatus.Ready)
                    this.GetOrCreate(f.Position.X, f.Position.Y).TryAdd(f);
                else
                    _annex.TryAdd(f);
            });
        }
        #endregion
    }
}
