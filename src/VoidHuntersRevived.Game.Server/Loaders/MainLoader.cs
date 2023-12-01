using Autofac;
using Guppy.Attributes;
using Guppy.Common.Extensions.Autofac;
using Guppy.Extensions.Autofac;
using Guppy.Loaders;
using Serilog;

namespace VoidHuntersRevived.Game.Server.Loaders
{
    [AutoLoad]
    internal sealed class MainLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder services)
        {
            services.Configure<LoggerConfiguration>((scope, config) =>
            {
                config.WriteTo.Console(outputTemplate: "[{PeerType}][{SimulationType}][{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
            });
        }
    }
}
