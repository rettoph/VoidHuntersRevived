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

        public SceneEntityCollection(ILogger logger, IScene scene) : base(logger)
        {
            _scene = scene;
            _entityLoader = _scene.Game.Provider.GetLoader<EntityLoader>();
        }

        /// <summary>
        /// Create a new entity and automatically add it to the current scene collection
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public TEntity Create<TEntity>()
            where TEntity : class, IEntity
        {
            return _entityLoader.Create<TEntity>(_scene);
        }

        protected override bool add(IEntity item)
        {
            if (base.add(item))
            {
                item.Scene = _scene;
                return true;
            }

            return false;
        }

        protected override bool remove(IEntity item)
        {
            if (base.remove(item))
            {
                item.Scene = null;
                return true;
            }

            return false;
        }
    }
}
