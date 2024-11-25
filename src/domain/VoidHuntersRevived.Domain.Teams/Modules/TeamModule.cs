using Autofac;
using Guppy.Core.Common.Extensions.Autofac;
using Serilog;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Pieces.Serialization.Json;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;
using VoidHuntersRevived.Domain.Teams.Common.Components;
using VoidHuntersRevived.Domain.Teams.Engines;
using VoidHuntersRevived.Domain.Teams.Services;

namespace VoidHuntersRevived.Domain.Teams.Modules
{
    public sealed class TeamModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<TeamJsonConverter>().As<JsonConverter>().SingleInstance();
            builder.RegisterType<ColorSchemeJsonConverter>().As<JsonConverter>().SingleInstance();

            builder.RegisterType<TeamService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterEngine<ColorSchemeEngine>();

            builder.Configure<LoggerConfiguration>((scope, config) =>
            {
                config.Destructure.AsScalar(typeof(Id<Team>));
            });
        }
    }
}
