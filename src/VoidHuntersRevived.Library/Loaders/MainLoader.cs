using Guppy.Attributes;
using Guppy.Loaders;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Loaders
{
    [AutoLoad]
    internal sealed class MainLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<LoggerConfiguration>(config =>
            {
                config.MinimumLevel.Is(Serilog.Events.LogEventLevel.Verbose);

                config.WriteTo.File($"logs/log_{DateTime.Now.ToString("yyyy-dd-M")}.txt")
                      .WriteTo.Console();
            });
        }
    }
}
