using Autofac;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Extensions.Autofac;
using Guppy.Core.Files.Common.Enums;
using Guppy.Core.Files.Common.Helpers;
using Guppy.Core.Files.Common.Services;
using Guppy.Engine.Common.Loaders;
using Serilog;

namespace VoidHuntersRevived.Presentation.Server.Loaders
{
    [AutoLoad]
    internal sealed class MainLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder services)
        {
            services.Configure<LoggerConfiguration>((scope, config) =>
            {
                if (scope.IsRoot())
                {
                    return;
                }

                var fileTypePaths = scope.Resolve<IPathService>();
                var source = fileTypePaths.GetSourceLocation(DirectoryType.AppData, "logs", $"log_{DateTime.Now.ToString("yyyy-dd-M")}.txt");
                DirectoryHelper.EnsureDirectoryExists(source);

                config
                    .WriteTo.File(
                        path: source.Path,
                        outputTemplate: "[{PeerType}][{SimulationType}][{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                        retainedFileCountLimit: 5,
                        shared: true
                    )
                    .WriteTo.Console(outputTemplate: "[{PeerType}][{SimulationType}][{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
            });
        }
    }
}
