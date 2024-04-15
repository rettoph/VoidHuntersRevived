using Autofac;
using Guppy.Core.Common.Attributes;
using Guppy.Engine.Common.Autofac;
using Guppy.Engine.Extensions.Autofac;
using Guppy.Core.Files.Enums;
using Guppy.Core.Files.Helpers;
using Guppy.Core.Files.Services;
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
                if (scope.HasTag(LifetimeScopeTags.GuppyScope))
                {
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
                }
            });
        }
    }
}
