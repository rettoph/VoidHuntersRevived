using Autofac;
using Guppy.Core.Common.Builders;
using Guppy.Core.Common.Extensions;
using Guppy.Core.Common.Extensions.System;
using Guppy.Core.Files.Common.Enums;
using Guppy.Core.Files.Common.Helpers;
using Guppy.Core.Files.Common.Services;
using Guppy.Core.Logging.Common.Enums;
using Guppy.Core.Logging.Common.Extensions;
using Guppy.Core.Network.Common.Enums;
using Guppy.Core.Network.Common.Extensions;
using Guppy.Game.Common.Extensions;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Strategies;
using VoidHuntersRevived.Domain.Teams.Common.Components;
using VoidHuntersRevived.Presentation.Core.Configurations;

namespace VoidHuntersRevived.Presentation.Core.Extensions
{
    public static class IGuppyRootBuilderExtensions
    {
        public static IGuppyRootBuilder RegisterPresentationCoreServices(this IGuppyRootBuilder builder)
        {
            builder.ConfigureLogger((scope, config) =>
            {
                Type? sceneType = scope.Variables.GetSceneType();
                if (sceneType is not null && sceneType.IsAssignableTo<IStrategy>())
                {
                    config.EnrichWith(nameof(StrategyTypeEnum), sceneType.GetFormattedName());
                }

                PeerTypeEnum peerTypeEnum = scope.Variables.GetPeerType();
                if (peerTypeEnum != PeerTypeEnum.None)
                {
                    config.EnrichWith(nameof(PeerTypeEnum), peerTypeEnum);
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

            builder.Configure<LoggerOutputTemplateConfiguration>((scope, config) =>
            {
                Type? sceneType = scope.Variables.GetSceneType();

                config.Value = sceneType is not null
                    ? $"[{{{nameof(PeerTypeEnum)}}}][{{{nameof(StrategyTypeEnum)}}}][{{Timestamp:HH:mm:ss}} {{Level:u3}}] {{SourceContext}} - {{Message:lj}}{{NewLine}}{{Exception}}"
                    : "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} - {Message:lj}{NewLine}{Exception}";
            });

            return builder.ConfigureFileLogMessageSink((scope, config) =>
            {
                IPathService pathService = scope.Resolve<IPathService>();
                string outputPath = pathService.GetFileSystemPath(DirectoryTypeEnum.AppData, "logs", $"log_{DateTime.Now:yyyy-dd-M}.txt");
                DirectoryHelper.EnsureDirectoryExists(outputPath);

                config.Enabled = true;
                config.OutputPath = outputPath;
                config.OutputTemplate = scope.GetLoggerOutputTemplate();
            });
        }
    }
}