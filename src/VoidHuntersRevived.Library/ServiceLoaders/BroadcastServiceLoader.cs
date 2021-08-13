using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Interfaces;
using Guppy.Network.Enums;
using Guppy.Network.Services;
using Guppy.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class BroadcastServiceLoader : IServiceLoader
    {
        public void RegisterServices(GuppyServiceCollection services)
        {
            services.RegisterSetup<Broadcasts>((broadcasts, provider, _) =>
            {
                Settings settings = provider.GetService<Settings>();

                switch (settings.Get<NetworkAuthorization>())
                {
                    case NetworkAuthorization.Master:
                        broadcasts.Register(Constants.Messages.WorldObject.WorldInfoPing, Constants.Intervals.PingPositionInterval);
                        break;
                }
            });
        }

        public void ConfigureProvider(GuppyServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
