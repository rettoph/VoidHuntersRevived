using Autofac;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Extensions.Autofac;
using Guppy.Core.Files.Common.Enums;
using Guppy.Core.Files.Common.Helpers;
using Guppy.Core.Files.Common.Services;
using Guppy.Core.Network.Enums;
using Guppy.Core.StateMachine.Common;
using Guppy.Core.StateMachine.Common.Services;
using Guppy.Engine.Common.Autofac;
using Guppy.Engine.Common.Loaders;
using Guppy.Engine.Extensions.Autofac;
using Guppy.Game.Common;
using Guppy.Game.Extensions.Serilog;
using Serilog;
using VoidHuntersRevived.Domain.Simulations.Common;

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
                        .WriteTo.Terminal(scope.Resolve<ITerminal>(), outputTemplate: "[{PeerType}][{SimulationType}][{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");

                    IStateService states = scope.Resolve<IStateService>();
                    config.Enrich.WithProperty("PeerType", states.GetByKey(StateKey<PeerType>.Create()).Value);
                    config.Enrich.WithProperty("SimulationType", states.GetByKey(StateKey<SimulationType>.Create()).Value);
                }
            });
        }
    }
}
