using Guppy;
using Guppy.Extensions.Collection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.ConnectionNodes;

namespace VoidHuntersRevived.Library.Utilities.Controllers
{
    public class ShipPartController : FarseerEntityController<ShipPart>
    {
        #region Private Attributes
        private List<ShipPart> _list;
        private ShipPart _root;
        private Boolean _dirty;
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _list = new List<ShipPart>();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            if(_dirty)
                this.SyncChain(_root);

            base.Update(gameTime);
        }
        #endregion

        #region Helper Methods
        public override bool Add(ShipPart entity)
        {
            if (base.Add(entity))
            {
                entity.Events.TryAdd<Creatable>("disposing", this.HandleComponentDisposing);

                return true;
            }

            return false;
        }

        public override bool Remove(ShipPart entity)
        {
            if (base.Remove(entity))
            {
                entity.Events.TryRemove<Creatable>("disposing", this.HandleComponentDisposing);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Automatically add new elements within the chain
        /// and remove any old elements no longer within
        /// </summary>
        /// <param name="root"></param>
        public void SyncChain(ShipPart root)
        {
            if(root == null)
            {
                while (this.Components.Any())
                    this.Remove(this.Components.First());

                this.logger.LogDebug($"Synced ShipPartController => Clear");
            }
            else
            {
                _list.Clear();
                root.GetAllChildren(ref _list);

                var removed = this.Components.Except(_list).ToList();
                var added = _list.Except(this.Components).ToList();

                this.logger.LogDebug($"Synced ShipPartController => Components: {this.Components.Count()}, Children: {_list.Count()}, Added: {added.Count()}, Removed: {removed.Count()}");

                // Add & remove children as needed
                removed.ForEach(sp => this.Remove(sp));
                added.ForEach(sp => this.Add(sp));
            }

            _dirty = false;
        }

        public void DirtyChain(ShipPart root)
        {
            _root = root;
            _dirty = true;
        }
        #endregion

        /// <summary>
        /// If an internal object is disposed, automatically remove it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private void HandleComponentDisposing(object sender, Creatable arg)
        {
            this.Remove(arg as ShipPart);
        }
    }
}
