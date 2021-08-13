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

            services.RegisterTypeFactory<LockService>(p => new LockService());
            services.RegisterTypeFactory<ShipPartShapesBuilderService>(p => new ShipPartShapesBuilderService());
            services.RegisterTypeFactory<ShipPartShapeBuilderService>(p => new ShipPartShapeBuilderService());
            services.RegisterTypeFactory<ShipPartShapeEditorService>(p => new ShipPartShapeEditorService());
            services.RegisterTypeFactory<ConnectionNodeEditorService>(p => new ConnectionNodeEditorService());
            services.RegisterTypeFactory<ShipPartOuterHullBuilderService>(p => new ShipPartOuterHullBuilderService());
            services.RegisterTypeFactory<ShipPartPropertiesEditorService>(p => new ShipPartPropertiesEditorService());

            services.RegisterTransient<LockService>();
            services.RegisterTransient<ShipPartShapesBuilderService>();
            services.RegisterTransient<ShipPartShapeBuilderService>();
            services.RegisterTransient<ShipPartShapeEditorService>();
            services.RegisterTransient<ConnectionNodeEditorService>();
            services.RegisterTransient<ShipPartPropertiesEditorService>();
            services.RegisterTransient<ShipPartOuterHullBuilderService>();
        }

        public void ConfigureProvider(GuppyServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
