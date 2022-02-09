using Guppy.Attributes;
using Guppy.EntityComponent.DependencyInjection.Builders;
using Guppy.ServiceLoaders;
using Microsoft.Xna.Framework;
using Serilog;
using System;

namespace VoidHuntersRevived.Client.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class SerilogLoader : ISerilogLoader
    {
        public void RegisterSerilog(LoggerConfiguration loggerConfiguration)
        {
            loggerConfiguration.WriteTo.Console();
        }
    }
}
