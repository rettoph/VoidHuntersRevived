using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Collections;
using VoidHuntersRevived.Core.Configurations;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;

namespace VoidHuntersRevived.Core.Factories
{
    /// <summary>
    /// Factory used to create scenes (and apply their configurations)
    /// </summary>
    public class SceneFactory : IFactory
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _provider;
        private struct SceneConfigurationContainer
        {
            public Type Type { get; set; }
            public ISceneConfiguration Configuration { get; set; }
        }
        private InitializableGameCollection<SceneConfigurationContainer> _configurations;

        public SceneFactory(ILogger logger, IServiceProvider provider)
        {
            _logger = logger;
            _provider = provider;
            _configurations = new InitializableGameCollection<SceneConfigurationContainer>(_logger);
        }

        public TScene Create<TScene>()
            where TScene : IScene
        {
            var type = typeof(TScene);
            _logger.LogDebug($"SceneFactory.Create<{type.Name}>()");

            TScene scene = (TScene)ActivatorUtilities.CreateInstance(_provider.CreateScope().ServiceProvider, type);

            // Apply each saved configuration that matches the current scene
            foreach (SceneConfigurationContainer confCon in _configurations)
                if (type.IsSubclassOf(confCon.Type) || confCon.Type.IsAssignableFrom(type))
                    confCon.Configuration.Configure(scene);

            // Initialize the scene
            scene.TryBoot();
            scene.TryPreInitialize();
            scene.TryInitialize();
            scene.TryPostInitialize();

            return scene;
        }

        public void ApplyConfiguration<TScene>(ISceneConfiguration configuration)
            where TScene : IScene
        {
            _configurations.Add(new SceneConfigurationContainer()
            {
                Type = typeof(TScene),
                Configuration = configuration
            });
        }
    }
}
