using Autofac;
using Guppy.Core.Common.Extensions.Autofac;
using Guppy.Core.Serialization.Common.Extensions;
using Serilog;
using Svelto.ECS;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Pieces.Serialization.Json;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;
using VoidHuntersRevived.Domain.Teams.Common.Components;
using VoidHuntersRevived.Domain.Teams.Engines;
using VoidHuntersRevived.Domain.Teams.Services;

namespace VoidHuntersRevived.Domain.Teams.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder RegisterDomainTeamsServices(this ContainerBuilder builder)
        {
            return builder.EnsureRegisteredOnce(nameof(RegisterDomainTeamsServices), builder =>
            {
                builder.RegisterJsonConverter<TeamJsonConverter>();
                builder.RegisterJsonConverter<ColorSchemeJsonConverter>();

                builder.RegisterPolymorphicJsonType<ColorScheme, IEntityComponent>(nameof(ColorScheme));
                builder.RegisterPolymorphicJsonType<Team, IEntityComponent>(nameof(Team));

                builder.RegisterType<TeamService>().AsImplementedInterfaces().InstancePerLifetimeScope();

                builder.RegisterEngine<ColorSchemeEngine>();

                builder.Configure<LoggerConfiguration>((scope, config) =>
                {
                    config.Destructure.AsScalar(typeof(Id<Team>));
                });
            });
        }
    }
}
