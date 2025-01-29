using Autofac;
using Guppy.Core.Common;
using Guppy.Core.Common.Extensions;
using Guppy.Core.Serialization.Common.Extensions;
using Guppy.Game.Common.Extensions;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Pieces.Serialization.Json;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;
using VoidHuntersRevived.Domain.Teams.Common.Components;
using VoidHuntersRevived.Domain.Teams.Engines;
using VoidHuntersRevived.Domain.Teams.Services;

namespace VoidHuntersRevived.Domain.Teams.Extensions
{
    public static class IGuppyScopeBuilderExtensions
    {
        public static IGuppyScopeBuilder RegisterDomainTeamsServices(this IGuppyScopeBuilder builder)
        {
            return builder.EnsureRegisteredOnce(nameof(RegisterDomainTeamsServices), builder =>
            {
                builder.RegisterJsonConverter<TeamJsonConverter>();
                builder.RegisterJsonConverter<ColorSchemeJsonConverter>();

                builder.RegisterPolymorphicJsonType<ColorScheme, IEntityComponent>(nameof(ColorScheme));
                builder.RegisterPolymorphicJsonType<Team, IEntityComponent>(nameof(Team));

                builder.RegisterSceneFilter<IStrategy>(builder =>
                {
                    builder.RegisterType<TeamService>().AsImplementedInterfaces().InstancePerLifetimeScope();

                    builder.RegisterEngine<ColorSchemeEngine>();
                });
            });
        }
    }
}