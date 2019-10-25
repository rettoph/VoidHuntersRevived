using Guppy;
using Guppy.Extensions.Collection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Collections;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.ConnectionNodes;

namespace VoidHuntersRevived.Library.Utilities.Controllers
{
    public class ShipPartController : BasicController<ShipPart>
    {
        #region Private Attributes
        private List<ShipPart> _list;
        private ChunkCollection _chunks;
        #endregion

        #region Constructor
        public ShipPartController(Annex annex, ChunkCollection chunks) : base(annex, chunks)
        {
            _chunks = chunks;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _list = new List<ShipPart>();

            this.Events.Register<IEnumerable<ShipPart>>("cleaned");
        }
        #endregion

        #region Frame Methods
        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.Components.ForEach(e => e.TryDraw(gameTime));
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Components.ForEach(e => e.TryUpdate(gameTime));
        }
        #endregion
        #endregion

        #region Helper Methods
        /// <summary>
        /// Automatically add new elements within the chain
        /// and remove any old elements not within
        /// </summary>
        /// <param name="root"></param>
        public void SyncChain(ShipPart root)
        {
            _list.Clear();
            if (root == null || root.Status != InitializationStatus.Ready)
            {
                if(this.Components.Any()) // Return all internal components to the chunk
                    _chunks.AddMany(this.Components.ToList());
            }
            else
            {
                root.GetAllChildren(_list);

                var removed = this.Components.Except(_list).ToList();
                var added = _list.Except(this.Components).ToList();

                // Add & remove children as needed
                _chunks.AddMany(removed);
                added.ForEach(sp => this.Add(sp));
            }
            _list.Clear();

            this.Events.TryInvoke<IEnumerable<ShipPart>>(this, "cleaned", this.Components);
        }
        #endregion
    }
}
