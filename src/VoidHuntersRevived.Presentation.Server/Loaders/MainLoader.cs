using Autofac;
using Guppy.Attributes;
using Guppy.Common.Autofac;
using Guppy.Extensions.Autofac;
using Guppy.Files.Enums;
using Guppy.Files.Helpers;
using Guppy.Files.Providers;
using Guppy.Loaders;
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
                if (scope.HasTag(LifetimeScopeTags.GuppyScope))
                {
                    var fileTypePaths = scope.Resolve<IFilePathProvider>();
                    var path = fileTypePaths.GetFullPath(FileType.AppData, Path.Combine("logs", $"log_{DateTime.Now.ToString("yyyy-dd-M")}.txt"));
                    DirectoryHelper.EnsureDirectoryExists(path);

                    config
                        .WriteTo.File(
                            path: path,
                            outputTemplate: "[{PeerType}][{SimulationType}][{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                            retainedFileCountLimit: 5,
                            shared: true
                        )
                        .WriteTo.Console(outputTemplate: "[{PeerType}][{SimulationType}][{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
                }
            });
        }
    }
}
