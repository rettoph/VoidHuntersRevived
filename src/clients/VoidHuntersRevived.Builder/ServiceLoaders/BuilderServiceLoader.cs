using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library;
using VoidHuntersRevived.Library.Scenes;
using Guppy.Extensions.DependencyInjection;
using VoidHuntersRevived.Builder.Scenes;
using VoidHuntersRevived.Library.Utilities;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Builder.UI;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Builder.ServiceLoaders
{
    [AutoLoad]
    internal sealed class BuilderServiceLoader : IServiceLoader
    {
        public void RegisterServices(ServiceCollection services)
        {
            services.AddGame<BuilderGame>(p => new BuilderGame(), 1);
            services.AddScene<ShipPartBuilderScene>(p => new ShipPartBuilderScene());

            services.AddFactory<SideInput>(p => new SideInput());
            services.AddTransient<SideInput>();
            services.AddFactory<ShapeTranslation>(p => new ShapeTranslation());
            services.AddTransient<ShapeTranslation>();

            services.AddTransient<RigidShipPart>("entity:ship-part:dynamic");
            services.AddSetup<RigidShipPart>("entity:ship-part:dynamic", (s, p, c) =>
            {
                s.Configuration = null;
            });

            services.AddSetup<Settings>((settings, p, c) =>
            {
                settings.Set<NetworkAuthorization>(NetworkAuthorization.Master);
                settings.Set<HostType>(HostType.Local);
            }, 100);
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
