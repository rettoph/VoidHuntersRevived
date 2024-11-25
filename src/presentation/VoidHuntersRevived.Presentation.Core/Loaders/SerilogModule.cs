using Autofac;
using Guppy.Core.Common;
using Guppy.Core.Common.Attributes;
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

namespace VoidHuntersRevived.Presentation.Core.Modules
{
    [AutoLoad]
    internal sealed class SerilogModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Configure<LoggerConfiguration>((scope, config) =>
            {
                IOptional<IStrategy> strategy = scope.Resolve<IOptional<IStrategy>>();

                string template = "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} - {Message:lj}{NewLine}{Exception}";
                if (strategy.HasValue)
                {
                    IStateService states = scope.Resolve<IStateService>();
                    config.Enrich.WithProperty(nameof(PeerType), states.GetByKey(StateKey<PeerType>.Create()));
                    config.Enrich.WithProperty(nameof(StrategyTypeEnum), states.GetByKey(StateKey<StrategyTypeEnum>.Create()));

                    template = $"[{{{nameof(PeerType)}}}][{{{nameof(StrategyTypeEnum)}}}][{{Timestamp:HH:mm:ss}} {{Level:u3}}] {{SourceContext}} - {{Message:lj}}{{NewLine}}{{Exception}}";
                }

                IPathService fileTypePaths = scope.Resolve<IPathService>();
                FileLocation source = fileTypePaths.GetSourceLocation(DirectoryType.AppData, "logs", $"log_{DateTime.Now.ToString("yyyy-dd-M")}.txt");
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
        }
    }
}
