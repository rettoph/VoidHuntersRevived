using Autofac;
using Guppy.Core.Common.Builders;
using Guppy.Core.Common.Extensions;
using Guppy.Core.Serialization.Common.Extensions;
using Guppy.Game.Common.Extensions;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Pieces.Serialization.Json;
using VoidHuntersRevived.Domain.Simulations.Common.Strategies;
using VoidHuntersRevived.Domain.Teams.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Services;
using VoidHuntersRevived.Domain.Teams.Services;
using VoidHuntersRevived.Domain.Teams.Systems;

namespace VoidHuntersRevived.Domain.Teams.Extensions
{
    public static class IGuppyRootBuilderExtensions
    {
        public static IGuppyRootBuilder RegisterDomainTeamsServices(this IGuppyRootBuilder builder)
        {
            return builder.EnsureRegisteredOnce(nameof(RegisterDomainTeamsServices), builder =>
            {
                builder.RegisterJsonConverter<TeamJsonConverter>();
                builder.RegisterJsonConverter<ColorSchemeJsonConverter>();

                builder.RegisterPolymorphicJsonType<ColorScheme, IEntityComponent>(nameof(ColorScheme));
                builder.RegisterPolymorphicJsonType<Team, IEntityComponent>(nameof(Team));

                builder.RegisterSceneFilter<IStrategy>(builder =>
                {
                    builder.RegisterType<TeamService>().AsSelf().As<ITeamService>().InstancePerLifetimeScope();

                    builder.RegisterSceneSystem<TeamServiceInitializationSystem>();
                    builder.RegisterSceneSystem<ColorSchemeSystem>();
                });
            });
        }
    }
}