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

            services.AddFactory<ShipPartShapesBuilderService>(p => new ShipPartShapesBuilderService());
            services.AddFactory<ShipPartShapeBuilderService>(p => new ShipPartShapeBuilderService());

            services.AddScoped<ShipPartShapesBuilderService>();
            services.AddScoped<ShipPartShapeBuilderService>();
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
