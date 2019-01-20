using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Factories;
using VoidHuntersRevived.Core.Interfaces;

namespace VoidHuntersRevived.Core.Collections
{
    public class SceneCollection : GameObjectCollection<IScene>
    {
        private SceneFactory _factory;
        private ILogger _logger;

        public SceneCollection(SceneFactory factory, ILogger logger)
            : base (logger)
        {
            _factory = factory;
            _logger = logger;
        }

        /// <summary>
        /// Wrapper for the SceneFactory.Create method,
        /// will automatically add the created scene to
        /// the current collection
        /// </summary>
        /// <typeparam name="TScene"></typeparam>
        public TScene Create<TScene>()
            where TScene : IScene
        {
            // Use the factory to create a new scene and add it to the collection
            return (TScene)this.Add(_factory.Create<TScene>());
        }
    }
}
