using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using VoidHuntersRevived.Core.Factories;
using VoidHuntersRevived.Core.Interfaces;

namespace VoidHuntersRevived.Core.Collections
{
    /// <summary>
    /// Stores a collection of layers within a scene
    /// </summary>
    public class LayerCollection : GameObjectCollection<ILayer>
    {
        private IScene _scene;
        private LayerFactory _factory;

        public LayerCollection(ILogger logger, IScene scene) : base(logger)
        {
            _scene = scene;
            _factory = new LayerFactory(logger, _scene);
        }

        /// <summary>
        /// Wrapper for the LayerFactory.Create method,
        /// will automatically add the created layer to
        /// the current collection
        /// </summary>
        /// <typeparam name="TLayer"></typeparam>
        public TLayer Create<TLayer>()
            where TLayer : class, ILayer
        {
            // Use the factory to create a new scene and add it to the collection
            return _factory.Create<TLayer>(_scene);
        }

        protected override bool add(ILayer item)
        {
            if(base.add(item))
            {
                item.Scene = _scene;

                return true;
            }

            return false;
        }

        protected override bool remove(ILayer item)
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
