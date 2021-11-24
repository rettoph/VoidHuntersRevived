using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Interfaces;
using Guppy.Network.Enums;
using Guppy.Network.Services;
using Guppy.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Globals.Constants;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Library.ServiceLoaders
{
    /// <summary>
    /// Create default broadcasts.
    /// </summary>
    [AutoLoad]
    internal sealed class BroadcastServiceLoader : IServiceLoader
    {
        public void RegisterServices(AssemblyHelper assemblyHelper, GuppyServiceCollection services)
        {
            services.RegisterSetup<Broadcasts>((broadcasts, provider, _) =>
            {
                if (provider.Settings.Get<HostType>() != HostType.Remote)
                    return;

                switch (provider.Settings.Get<NetworkAuthorization>())
                {
                    case NetworkAuthorization.Master:
                        broadcasts.Register(Messages.WorldObject.WorldInfoPing, Intervals.WorldInfoPingBroadcastInterval);
                        broadcasts.Register(Messages.Ship.TargetChanged, Intervals.ShipTargetPingBroadcastInterval);
                        break;
                    case NetworkAuthorization.Slave:
                        broadcasts.Register(Messages.UserPlayer.RequestTargetChangedAction, Intervals.ShipTargetPingBroadcastInterval);
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
