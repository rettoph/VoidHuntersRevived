using Autofac;
using Guppy.Attributes;
using Guppy.Common.Autofac;
using Guppy.Extensions.Autofac;
using Guppy.Files.Enums;
using Guppy.Files.Helpers;
using Guppy.Files.Providers;
using Guppy.Game.Common;
using Guppy.Game.Extensions.Serilog;
using Guppy.Loaders;
using Serilog;

namespace VoidHuntersRevived.Presentation.Client.Loaders
{
    [AutoLoad]
    internal class MainLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder services)
        {
            services.Configure<LoggerConfiguration>((scope, config) =>
            {
                if (scope.HasTag(LifetimeScopeTags.GuppyScope))
                {
                    var fileTypePaths = scope.Resolve<IPathProvider>();
                    var source = fileTypePaths.GetSourceLocation(DirectoryType.AppData, "logs", $"log_{DateTime.Now.ToString("yyyy-dd-M")}.txt");
                    DirectoryHelper.EnsureDirectoryExists(source);

                    config
                        .WriteTo.File(
                            path: source.Path,
                            outputTemplate: "[{PeerType}][{SimulationType}][{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                            retainedFileCountLimit: 5,
                            shared: true
                        )
                        .WriteTo.Terminal(scope.Resolve<ITerminal>(), outputTemplate: "[{PeerType}][{SimulationType}][{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
                }
            });
        }
    }
}
