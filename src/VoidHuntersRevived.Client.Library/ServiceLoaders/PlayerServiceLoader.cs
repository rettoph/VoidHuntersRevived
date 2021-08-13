using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Components.Entities.Players;
using VoidHuntersRevived.Library.Entities.Players;

namespace VoidHuntersRevived.Client.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class PlayerServiceLoader : IServiceLoader
    {
        public void RegisterServices(GuppyServiceCollection services)
        {
            services.RegisterTypeFactory<UserPlayerLocalComponent>(p => new UserPlayerLocalComponent());

            services.RegisterTransient<UserPlayerLocalComponent>();

            services.RegisterComponent<UserPlayerLocalComponent, UserPlayer>();
        }

        public void ConfigureProvider(GuppyServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
