using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using VoidHuntersRevived.Core.Interfaces;

namespace VoidHuntersRevived.Core.Collections
{
    /// <summary>
    ///  A collection of entities as they reside within the current layer.
    ///  Unlike scene entity collections, this collection contains all
    ///  entities within a specific layer
    /// </summary>
    public class LayerEntityCollection : GameObjectCollection<IEntity>
    {
        private ILayer _layer;

        public LayerEntityCollection(ILogger logger, ILayer layer) : base(logger)
        {
            _layer = layer;
        }

        /// <summary>
        /// Create a new entity and automatically add it to the current layer collection
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public TEntity Create<TEntity>(String handle, ILayer layer = null)
            where TEntity : class, IEntity
        {
            var entity = _layer.Scene.Entities.Create<TEntity>(handle, _layer);

            return entity;
        }

        protected override bool add(IEntity item)
        {
            if (base.add(item))
            {
                item.Layer = _layer;

                item.OnRemovedFromLayer += this.HandleEntityRemovedFromLayer;

                return true;
            }

            return false;
        }

        protected override bool remove(IEntity item)
        {
            if (base.remove(item))
            {
                item.Layer = null;

                item.OnRemovedFromLayer -= this.HandleEntityRemovedFromLayer;

                return true;
            }

            return false;
        }

        #region Event Handlers
        private void HandleEntityRemovedFromLayer(object sender, ILayerObject e)
        {
            var entity = (IEntity)sender;
            this.remove(entity);
        }
        #endregion
    }
}
