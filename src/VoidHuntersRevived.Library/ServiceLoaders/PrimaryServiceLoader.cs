using Guppy.Attributes;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.EntityComponent.DependencyInjection.Builders;
using Guppy.Interfaces;
using Guppy.ServiceLoaders;
using Guppy.Utilities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class PrimaryServiceLoader : IServiceLoader, ISerilogLoader
    {
        public void RegisterSerilog(LoggerConfiguration loggerConfiguration)
        {
            loggerConfiguration.MinimumLevel.Is(Serilog.Events.LogEventLevel.Information);
        }

        public void RegisterServices(AssemblyHelper assemblyHelper, ServiceProviderBuilder services)
        {
            services.RegisterGame<PrimaryGame>();
            services.RegisterScene<PrimaryScene>();
        }
    }
}
