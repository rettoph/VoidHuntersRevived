using Autofac;
using Guppy.Attributes;
using Guppy.Common.Extensions.Autofac;
using Guppy.Files.Enums;
using Guppy.Files.Helpers;
using Guppy.Files.Providers;
using Guppy.Loaders;
using Serilog;
using VoidHuntersRevived.Domain.GameComponents;

namespace VoidHuntersRevived.Domain.Loaders
{
    [AutoLoad]
    public sealed class DomainLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder services)
        {
            services.Configure<LoggerConfiguration>((scope, config) =>
            {
                var fileTypePaths = scope.Resolve<IFileTypePathProvider>();
                var path = fileTypePaths.GetFullPath(FileType.AppData, Path.Combine("logs", $"log_{DateTime.Now.ToString("yyyy-dd-M")}.txt"));
                DirectoryHelper.EnsureDirectoryExists(path);

                config.MinimumLevel.Is(Serilog.Events.LogEventLevel.Verbose);

                config
                    .WriteTo.File(
                        path: path, 
                        outputTemplate: "[{PeerType}][{SimulationType}][{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
                    )
                    .WriteTo.Console(outputTemplate: "[{PeerType}][{SimulationType}][{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
            });


            services.RegisterType<LaunchComponent>().AsSelf().AsImplementedInterfaces().InstancePerLifetimeScope();
        }
    }
}
