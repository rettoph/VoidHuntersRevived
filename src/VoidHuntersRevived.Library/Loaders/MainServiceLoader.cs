using Guppy.Attributes;
using Guppy.Common;
using Guppy.Filters;
using Guppy.Loaders;
using Guppy.Network.Enums;
using Guppy.Resources.Filters;
using Guppy.Resources.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Factories;
using VoidHuntersRevived.Library.GameComponents;
using VoidHuntersRevived.Library.Providers;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Systems;

namespace VoidHuntersRevived.Library.Loaders
{
    [AutoLoad]
    internal sealed class MainServiceLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGameComponent<AetherGameComponent>();

            services.AddScoped<IStepService, StepService>()
                    .AddAlias<IGameComponent, IStepService>();

            services.AddScoped<StepLocalProvider>()
                    .AddScoped<StepRemoteProvider>()
                    .AddAliases(Alias.ForMany<IStepProvider>(typeof(StepLocalProvider), typeof(StepRemoteProvider)))
                    .AddFilter(new SettingFilter<NetAuthorization, StepLocalProvider>(NetAuthorization.Master))
                    .AddFilter(new SettingFilter<NetAuthorization, StepRemoteProvider>(NetAuthorization.Slave));

            services.AddScoped<ITickService, TickService>()
                    .AddScoped<ITickFactory, TickFactory>();

            services.AddScoped<TickLocalProvider>()
                    .AddScoped<TickRemoteProvider>()
                    .AddAliases(Alias.ForMany<ITickProvider>(typeof(TickLocalProvider), typeof(TickRemoteProvider)))
                    .AddFilter(new SettingFilter<NetAuthorization, TickLocalProvider>(NetAuthorization.Master))
                    .AddFilter(new SettingFilter<NetAuthorization, TickRemoteProvider>(NetAuthorization.Slave));
        }
    }
}
