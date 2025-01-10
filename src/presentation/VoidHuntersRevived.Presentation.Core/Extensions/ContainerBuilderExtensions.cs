using Autofac;
using Guppy.Core.Common;
using Guppy.Core.Common.Extensions.Autofac;
using Guppy.Core.Files.Common;
using Guppy.Core.Files.Common.Enums;
using Guppy.Core.Files.Common.Helpers;
using Guppy.Core.Files.Common.Services;
using Guppy.Core.Network.Common.Enums;
using Guppy.Core.StateMachine.Common;
using Guppy.Core.StateMachine.Common.Services;
using Serilog;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Presentation.Core.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder RegisterPresentationCoreServices(this ContainerBuilder builder) => builder.EnsureRegisteredOnce(nameof(RegisterPresentationCoreServices), builder =>
                                                                                                                   {
                                                                                                                       builder.Configure<LoggerConfiguration>((scope, config) =>
                                                                                                                       {
                                                                                                                           IOptional<IStrategy> strategy = scope.Resolve<IOptional<IStrategy>>();

                                                                                                                           string template = "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} - {Message:lj}{NewLine}{Exception}";
                                                                                                                           if (strategy.HasValue)
                                                                                                                           {
                                                                                                                               IStateService states = scope.Resolve<IStateService>();
                                                                                                                               config.Enrich.WithProperty(nameof(PeerTypeEnum), states.GetByKey(StateKey<PeerTypeEnum>.Create()));
                                                                                                                               config.Enrich.WithProperty(nameof(StrategyTypeEnum), states.GetByKey(StateKey<StrategyTypeEnum>.Create()));

                                                                                                                               template = $"[{{{nameof(PeerTypeEnum)}}}][{{{nameof(StrategyTypeEnum)}}}][{{Timestamp:HH:mm:ss}} {{Level:u3}}] {{SourceContext}} - {{Message:lj}}{{NewLine}}{{Exception}}";
                                                                                                                           }

                                                                                                                           IPathService fileTypePaths = scope.Resolve<IPathService>();
                                                                                                                           FileLocation source = fileTypePaths.GetSourceLocation(DirectoryTypeEnum.AppData, "logs", $"log_{DateTime.Now:yyyy-dd-M}.txt");
                                                                                                                           DirectoryHelper.EnsureDirectoryExists(source);

                                                                                                                           config.WriteTo.File(
                                                                                                                               path: source.Path,
                                                                                                                               outputTemplate: template,
                                                                                                                               retainedFileCountLimit: 5,
                                                                                                                               shared: true
                                                                                                                           );

                                                                                                                           ISerilogSinkConfigurator configurator = scope.Resolve<ISerilogSinkConfigurator>();
                                                                                                                           configurator.Configure(config, template);
                                                                                                                       });
                                                                                                                   });
    }
}