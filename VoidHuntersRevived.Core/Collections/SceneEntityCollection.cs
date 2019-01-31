using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using VoidHuntersRevived.Core.Extensions;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Loaders;

namespace VoidHuntersRevived.Core.Collections
{
    /// <summary>
    ///  A collection of entities as they reside within the current scene.
    ///  Unlike layer entity collections, this collection contains all
    ///  entities within a scene no matter what layer (if any) they 
    ///  are currently on.
    /// </summary>
    public class SceneEntityCollection : GameObjectCollection<IEntity>
    {
        private IScene _scene;
        private EntityLoader _entityLoader;
        private ILayer _defaultLayer;
        private ILogger _logger;

        public SceneEntityCollection(ILogger logger, IScene scene) : base(logger)
        {
            _scene = scene;
            _entityLoader = _scene.Game.Provider.GetLoader<EntityLoader>();
            _logger = logger;
        }

        public void SetDefaultLayer(ILayer layer)
        {
            _defaultLayer = layer;
        }

        /// <summary>
        /// Create a new entity and automatically add it to the current scene collection
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public TEntity Create<TEntity>(String handle, ILayer layer = null, params object[] parameters)
            where TEntity : class, IEntity
        {
            var entity = _entityLoader.Create<TEntity>(handle, _scene, parameters);
            entity.Layer = layer ?? _defaultLayer;

            return entity;
        }

        protected override bool add(IEntity item)
        {
            if (base.add(item))
            {
                item.Scene = _scene;

                item.OnAddedToLayer += this.HandleEntityAddedToLayer;

                return true;
            }

            return false;
        }

        protected override bool remove(IEntity item)
        {
            if (base.remove(item))
            {
                _logger.LogDebug($"Removing IEntity<{item.GetType().Name}> from SceneEntityCollection.");

                item.Layer = null;
                item.OnAddedToLayer -= this.HandleEntityAddedToLayer;


                item.Dispose();

                return true;
            }

            return false;
        }

        #region Event Handlers 
        private void HandleEntityAddedToLayer(object sender, ILayerObject e)
        {
            // Add the entity to its new layer
            var entity = sender as IEntity;
            entity.Layer?.Entities.Add(entity);
        }
        #endregion
    }
}
