using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VoidHuntersRevived.Core.Interfaces;

namespace VoidHuntersRevived.Core.Collections
{
    public class SceneServiceCollection : GameObjectCollection<ISceneService>
    {
        private IScene _scene;

        public SceneServiceCollection(ILogger logger, IScene scene) : base(logger)
        {
            _scene = scene;
        }

        protected override bool add(ISceneService item)
        {
            if (base.add(item))
            {
                item.Scene = _scene;

                return true;
            }

            return false;
        }

        protected override bool remove(ISceneService item)
        {
            if (base.remove(item))
            {
                item.Dispose();

                return true;
            }

            return false;
        }

        /// <summary>
        /// will automatically create scene service and
        /// add it to the current collection
        /// </summary>
        /// <typeparam name="TSceneService"></typeparam>
        public TSceneService Create<TSceneService>()
            where TSceneService : ISceneService
        {
            // Use the factory to create a new scene and add it to the collection
            return (TSceneService)this.Add(
                (TSceneService)ActivatorUtilities.CreateInstance(_scene.Provider, typeof(TSceneService)));
        }
    }
}
