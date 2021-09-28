using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Components.Entities.Players;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class PlayerServiceLoader : IServiceLoader
    {
        public void RegisterServices(GuppyServiceCollection services)
        {
            services.RegisterTypeFactory<PlayerService>(p => new PlayerService());
            services.RegisterTypeFactory<UserPlayer>(p => new UserPlayer());

            services.RegisterScoped<PlayerService>();
            services.RegisterTransient(Constants.ServiceConfigurationKeys.Players.UserPlayer, typeof(UserPlayer));

            #region Components
            services.RegisterTypeFactory<PlayerPipeComponent>(p => new PlayerPipeComponent());
            services.RegisterTypeFactory<UserPlayerMasterCRUDComponent>(p => new UserPlayerMasterCRUDComponent());
            services.RegisterTypeFactory<UserPlayerSlaveCRUDComponent>(p => new UserPlayerSlaveCRUDComponent());
            services.RegisterTypeFactory<UserPlayerCurrentUserDirectionComponent>(p => new UserPlayerCurrentUserDirectionComponent());
            services.RegisterTypeFactory<UserPlayerCurrentUserTractorBeamComponent>(p => new UserPlayerCurrentUserTractorBeamComponent());
            services.RegisterTypeFactory<UserPlayerCurrentUserTargetingComponent>(p => new UserPlayerCurrentUserTargetingComponent());

            services.RegisterTransient<PlayerPipeComponent>();
            services.RegisterTransient<UserPlayerMasterCRUDComponent>();
            services.RegisterTransient<UserPlayerSlaveCRUDComponent>();
            services.RegisterTransient<UserPlayerCurrentUserDirectionComponent>();
            services.RegisterTransient<UserPlayerCurrentUserTractorBeamComponent>();
            services.RegisterTransient<UserPlayerCurrentUserTargetingComponent>();

            services.RegisterComponent<PlayerPipeComponent, Player>();
            services.RegisterComponent<UserPlayerMasterCRUDComponent, UserPlayer>();
            services.RegisterComponent<UserPlayerSlaveCRUDComponent, UserPlayer>();
            services.RegisterComponent<UserPlayerCurrentUserDirectionComponent, UserPlayer>();
            services.RegisterComponent<UserPlayerCurrentUserTractorBeamComponent, UserPlayer>();
            services.RegisterComponent<UserPlayerCurrentUserTargetingComponent, UserPlayer>();
            #endregion
        }

        public void ConfigureProvider(GuppyServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
