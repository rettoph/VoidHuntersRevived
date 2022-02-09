using Guppy.Attributes;
using Guppy.EntityComponent.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using Guppy.EntityComponent.DependencyInjection.Builders;
using Guppy.ServiceLoaders;
using Serilog;

namespace VoidHuntersRevived.Server.ServiceLoaders
{
    [AutoLoad]
    internal sealed class SerilogLoader : ISerilogLoader
    {
        public void RegisterSerilog(LoggerConfiguration loggerConfiguration)
        {
            loggerConfiguration
                .WriteTo.Console()
                .WriteTo.File("logs/vhr.txt", rollingInterval: RollingInterval.Day);
        }
    }
}
