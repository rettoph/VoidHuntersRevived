using Autofac;
using Guppy.Attributes;
using Guppy.Common.Extensions.Autofac;
using Guppy.Extensions.Autofac;
using Guppy.Loaders;
using Guppy.MonoGame;
using Guppy.MonoGame.Extensions.Serilog;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Domain.Client.Services;

namespace VoidHuntersRevived.Domain.Client.Loaders
{
    [AutoLoad]
    public sealed class ClientLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder services)
        {
            services.RegisterType<VisibleRenderingService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            services.Configure<LoggerConfiguration>((scope, config) =>
            {
                try
                {
                    var terminal = scope.Resolve<ITerminal>();
                    config.WriteTo.Terminal(terminal, "[{PeerType}][{SimulationType}][{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
                }
                catch
                {
                    config.WriteTo.Console(outputTemplate: "[{PeerType}][{SimulationType}][{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
                }
            });
        }
    }
}
