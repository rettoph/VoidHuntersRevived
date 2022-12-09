using Guppy.Attributes;
using Guppy.Common;
using Guppy.Filters;
using Guppy.Loaders;
using Guppy.Network.Enums;
using Guppy.Resources.Filters;
using Guppy.Resources.Providers;
using Guppy.Resources.Serialization.Json.Converters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Factories;
using VoidHuntersRevived.Library.GameComponents;
using VoidHuntersRevived.Library.Messages;
using VoidHuntersRevived.Library.Providers;
using VoidHuntersRevived.Library.Serialization.Json.Converters;
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

            services.AddScoped<GameState>();

            services.AddScoped<StepLocalProvider>()
                    .AddScoped<StepRemoteProvider>()
                    .AddAliases(Alias.ManyFor<IStepProvider>(typeof(StepLocalProvider), typeof(StepRemoteProvider)))
                    .AddFilter(new SettingFilter<NetAuthorization, StepLocalProvider>(NetAuthorization.Master))
                    .AddFilter(new SettingFilter<NetAuthorization, StepRemoteProvider>(NetAuthorization.Slave));

            services.AddScoped<ITickService, TickService>()
                    .AddScoped<ITickFactory, TickFactory>();

            services.AddScoped<TickLocalProvider>()
                    .AddScoped<TickRemoteProvider>()
                    .AddAliases(Alias.ManyFor<ITickProvider>(typeof(TickLocalProvider), typeof(TickRemoteProvider)))
                    .AddFilter(new SettingFilter<NetAuthorization, TickLocalProvider>(NetAuthorization.Master))
                    .AddFilter(new SettingFilter<NetAuthorization, TickRemoteProvider>(NetAuthorization.Slave));

            services.AddSingleton<JsonConverter, PolymorphicJsonConverter<ITickData>>()
                    .AddSingleton<JsonConverter, TickJsonConverter>();

            services.Configure<LoggerConfiguration>(config =>
            {
                config.MinimumLevel.Is(Serilog.Events.LogEventLevel.Verbose);

                config.WriteTo.File($"logs/log_{DateTime.Now.ToString("yyyy-dd-M")}.txt")
                      .WriteTo.Console();
            });
        }
    }
}
