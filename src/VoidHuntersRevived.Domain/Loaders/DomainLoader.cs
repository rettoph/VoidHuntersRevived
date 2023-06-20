using Guppy.Attributes;
using Guppy.Common.DependencyInjection;
using Guppy.Loaders;
using Guppy.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.GameComponents;

namespace VoidHuntersRevived.Domain.Loaders
{
    [AutoLoad]
    public sealed class DomainLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<LoggerConfiguration>(config =>
            {
                config.MinimumLevel.Is(Serilog.Events.LogEventLevel.Verbose);

                config.WriteTo.File($"logs/log_{DateTime.Now.ToString("yyyy-dd-M")}.txt")
                      .WriteTo.Console();
            });

            services.ConfigureCollection(manager =>
            {
                manager.AddScoped<LaunchComponent>()
                    .AddInterfaceAliases();
            });
        }
    }
}
