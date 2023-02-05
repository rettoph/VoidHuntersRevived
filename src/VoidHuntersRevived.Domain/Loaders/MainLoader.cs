using Guppy.Attributes;
using Guppy.Loaders;
using Guppy.MonoGame.Resources;
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

namespace VoidHuntersRevived.Domain.Loaders
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

            services.AddSingleton<Pack>(new Pack(VoidHuntersPack.Id, VoidHuntersPack.Name)
            {
                Directory = VoidHuntersPack.Directory
            }.Add(new ColorResource(Colors.Orange, Color.Orange)));
        }
    }
}
