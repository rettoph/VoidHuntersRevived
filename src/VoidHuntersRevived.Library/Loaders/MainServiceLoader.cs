using Guppy.Attributes;
using Guppy.Common;
using Guppy.Common.DependencyInjection;
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
using VoidHuntersRevived.Library.Games;
using VoidHuntersRevived.Library.Messages;
using VoidHuntersRevived.Library.Providers;
using VoidHuntersRevived.Library.Serialization.Json.Converters;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Simulations.EventData;

namespace VoidHuntersRevived.Library.Loaders
{
    [AutoLoad(0)]
    internal sealed class MainServiceLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFilter(new SettingFilter<NetAuthorization, StepLocalProvider>(NetAuthorization.Master))
                    .AddFilter(new SettingFilter<NetAuthorization, StepRemoteProvider>(NetAuthorization.Slave));

            services.AddScoped<ITickService, TickService>()
                    .AddScoped<ITickFactory, TickFactory>();

            services.ConfigureCollection(manager =>
                    {
                        manager.AddScoped<TickLocalProvider>()
                            .AddInterfaceAliases();

                        manager.AddScoped<TickRemoteProvider>()
                            .AddInterfaceAliases();
                    })
                    .AddFilter(new SettingFilter<NetAuthorization, TickLocalProvider>(NetAuthorization.Master))
                    .AddFilter(new SettingFilter<NetAuthorization, TickRemoteProvider>(NetAuthorization.Slave));

            services.AddSingleton<JsonConverter, PolymorphicJsonConverter<ISimulationEventData>>()
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
