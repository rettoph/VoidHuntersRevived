using Autofac;
using Guppy.Core.Common;
using Guppy.Core.Files.Common;
using Guppy.Core.Files.Common.Enums;
using Guppy.Core.Files.Common.Helpers;
using Guppy.Core.Files.Common.Services;
using Guppy.Core.Logging.Common.Enums;
using Guppy.Core.Logging.Common.Extensions;
using Guppy.Core.Network.Common.Enums;
using Guppy.Core.StateMachine.Common;
using Guppy.Core.StateMachine.Common.Services;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Presentation.Core.Extensions
{
    public static class IGuppyScopeBuilderExtensions
    {
        public static IGuppyScopeBuilder RegisterPresentationCoreServices(this IGuppyScopeBuilder builder)
        {
            builder.ConfigureLogger((scope, config) =>
            {
                IOptional<IStrategy> strategy = scope.Resolve<IOptional<IStrategy>>();
                if (strategy.HasValue)
                {
                    IStateService states = scope.Resolve<IStateService>();
                    config.EnrichWith(nameof(PeerTypeEnum), states.GetByKey(StateKey<PeerTypeEnum>.Create()));
                    config.EnrichWith(nameof(StrategyTypeEnum), states.GetByKey(StateKey<StrategyTypeEnum>.Create()));
                }

                config.SetParameterType(LogMessageParameterTypeEnum.Scalar, [
                    typeof(EntityLocalId),
                    typeof(EntityGlobalId),
                    typeof(VhId),
                    typeof(Id<Blueprint>),
                    typeof(Id<Team>),
                    typeof(Id<IEntityComponent>),
                    typeof(Id<EntityTemplateFragment>)
                ]);
            });

            return builder.ConfigureFileLogMessageSink((scope, config) =>
            {
                IOptional<IStrategy> strategy = scope.Resolve<IOptional<IStrategy>>();
                string outputTemplate = strategy.HasValue == true
                    ? $"[{{{nameof(PeerTypeEnum)}}}][{{{nameof(StrategyTypeEnum)}}}][{{Timestamp:HH:mm:ss}} {{Level:u3}}] {{SourceContext}} - {{Message:lj}}{{NewLine}}{{Exception}}"
                    : "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} - {Message:lj}{NewLine}{Exception}";

                IPathService fileTypePaths = scope.Resolve<IPathService>();
                FileLocation source = fileTypePaths.GetSourceLocation(DirectoryTypeEnum.AppData, "logs", $"log_{DateTime.Now:yyyy-dd-M}.txt");
                DirectoryHelper.EnsureDirectoryExists(source);

                config.Enabled = true;
                config.Path = source;
                config.OutputTemplate = outputTemplate;
            });
        }
    }
}