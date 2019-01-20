using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;

namespace VoidHuntersRevived.Core.Factories
{
    public class LayerFactory
    {
        private readonly ILogger _logger;
        private readonly IScene _scene;

        public LayerFactory(ILogger logger, IScene scene)
        {
            _logger = logger;
            _scene = scene;
        }

        public TLayer Create<TLayer>()
            where TLayer : ILayer
        {
            var type = typeof(TLayer);
            _logger.LogDebug($"LayerFactory.Create<{type.Name}>()");

            TLayer layer = (TLayer)ActivatorUtilities.CreateInstance(_scene.Provider, type);

            // Initialize the scene
            layer.TryBoot();
            layer.TryPreInitialize();
            layer.TryInitialize();
            layer.TryPostInitialize();

            return layer;
        }
    }
}
