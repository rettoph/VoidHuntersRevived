using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Interfaces;
using Guppy.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Builder.Scenes;
using VoidHuntersRevived.Builder.Services;

namespace VoidHuntersRevived.Builder.ServiceLoaders
{
    [AutoLoad]
    internal sealed class BuilderServiceLoader : IServiceLoader
    {
        public void RegisterServices(ServiceCollection services)
        {
            services.AddGame<BuilderGame>(p => new BuilderGame());
            services.AddScene<BuilderScene>(p => new BuilderScene());

            services.AddFactory<LockService>(p => new LockService());
            services.AddFactory<ShipPartShapesBuilderService>(p => new ShipPartShapesBuilderService());
            services.AddFactory<ShipPartShapeBuilderService>(p => new ShipPartShapeBuilderService());
            services.AddFactory<ShipPartShapeEditorService>(p => new ShipPartShapeEditorService());
            services.AddFactory<ConnectionNodeEditorService>(p => new ConnectionNodeEditorService());
            services.AddFactory<ShipPartPropertiesEditorService>(p => new ShipPartPropertiesEditorService());

            services.AddTransient<LockService>();
            services.AddTransient<ShipPartShapesBuilderService>();
            services.AddTransient<ShipPartShapeBuilderService>();
            services.AddTransient<ShipPartShapeEditorService>();
            services.AddTransient<ConnectionNodeEditorService>();
            services.AddTransient<ShipPartPropertiesEditorService>();
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
